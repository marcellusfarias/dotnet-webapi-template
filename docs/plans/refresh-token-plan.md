# Refresh Token Implementation Plan

## Overview

Add refresh token support so clients can obtain new access tokens without re-authenticating.
The access token remains short-lived (configured in `JwtSettings.ExpirationTime`); the refresh
token is long-lived, opaque, single-use, and stored (hashed) in the database.

---

## Design Decisions

| Decision | Choice | Reason |
|---|---|---|
| Token format | Cryptographically random bytes, base64url-encoded | Opaque, unforgeable, no claims leakage |
| DB storage | SHA-256 hash of the raw token | Raw token never at rest; hash alone is useless to an attacker |
| Rotation | New refresh token issued on every use (old one revoked) | Limits replay window; theft becomes detectable |
| Reuse detection | `IsRevoked` flag; reuse of a revoked token revokes all tokens for that user + logs a security warning | Detects token theft |
| Expiry | Configurable via `RefreshTokenOptions.ExpirationDays` | Flexible per environment |
| Response shape | Change `POST /api/auth` from `text/plain` string → JSON `LoginResponse` | Must carry both tokens and expiration timestamps |
| Expiration format | `DateTimeOffset` (ISO 8601 with timezone offset) | Client gets unambiguous absolute time without clock arithmetic |
| New endpoint | `POST /api/auth/refresh` | Issues new access + refresh token pair |
| Token cleanup | `BackgroundService` runs daily; deletes expired tokens + revoked tokens older than 30 days | Bounded disk usage; short audit window retained for security forensics |

---

## Files to Create

### Domain
- `MyBuyingList.Domain/Entities/RefreshToken.cs`
  ```
  Id, UserId, TokenHash (string), ExpiresAt (DateTimeOffset), CreatedAt (DateTimeOffset), IsRevoked (bool)
  Navigation: User
  ```

### Application
- `MyBuyingList.Application/Features/Login/DTOs/LoginResponse.cs`
  — replaces the raw `string` return:
  ```
  AccessToken (string)
  AccessTokenExpiresAt (DateTimeOffset)
  RefreshToken (string)
  RefreshTokenExpiresAt (DateTimeOffset)
  ```

- `MyBuyingList.Application/Features/Login/DTOs/RefreshRequest.cs`
  — single `RefreshToken` string property

- `MyBuyingList.Application/Features/Login/Validators/RefreshRequestValidator.cs`
  — `NotEmpty()` rule for `RefreshToken`

- `MyBuyingList.Application/Common/Interfaces/IRefreshTokenRepository.cs`
  ```
  Task<RefreshToken?> GetByTokenHashAsync(string hash, CancellationToken ct)
  Task AddAsync(RefreshToken token, CancellationToken ct)
  Task RevokeAsync(RefreshToken token, CancellationToken ct)
  Task RevokeAllForUserAsync(int userId, CancellationToken ct)
  Task DeleteExpiredAndRevokedAsync(DateTimeOffset revokedBefore, CancellationToken ct)
  ```

- `MyBuyingList.Application/Common/Options/RefreshTokenOptions.cs`
  ```
  ExpirationDays (int)
  RevokedTokenRetentionDays (int)   // how long to keep revoked tokens before hard delete; default 30
  ```

- `MyBuyingList.Application/Features/Login/Services/IRefreshTokenService.cs`
  ```
  Task<LoginResponse> RefreshAsync(RefreshRequest request, CancellationToken ct)
  ```

- `MyBuyingList.Application/Features/Login/Services/RefreshTokenService.cs`
  — validates token hash, checks expiry + revocation, rotates token, returns new `LoginResponse`
  — on reuse of a revoked token: log `LogWarning` with userId + sanitized token prefix, then revoke all user tokens

### Infrastructure
- `MyBuyingList.Infrastructure/Persistence/Configurations/RefreshTokenConfiguration.cs`
  — maps `refresh_tokens` table; `token_hash` is unique index, max length = 64 chars (SHA-256 hex)

- `MyBuyingList.Infrastructure/Repositories/RefreshTokenRepository.cs`
  — EF Core implementation of `IRefreshTokenRepository`

- `MyBuyingList.Infrastructure/BackgroundServices/RefreshTokenCleanupService.cs`
  — `BackgroundService` that runs once every 24 hours
  — calls `IRefreshTokenRepository.DeleteExpiredAndRevokedAsync(DateTimeOffset.UtcNow.AddDays(-RevokedTokenRetentionDays))`
  — logs how many rows were deleted at `LogInformation` level

- EF Core migration (auto-generated via `dotnet ef migrations add AddRefreshTokens ...`)

---

## Files to Modify

### Domain
- `MyBuyingList.Domain/Entities/User.cs`
  — add `ICollection<RefreshToken> RefreshTokens { get; set; } = []` navigation property

- `MyBuyingList.Domain/Constants/FieldLengths.cs`
  — add `REFRESH_TOKEN_HASH_MAX_LENGTH = 64`

### Application
- `MyBuyingList.Application/Features/Login/Services/ILoginService.cs`
  — return type `Task<string>` → `Task<LoginResponse>`

- `MyBuyingList.Application/Features/Login/Services/LoginService.cs`
  — after generating the JWT, also create a refresh token (random bytes → store hash → return raw)
  — return `LoginResponse` (with both tokens and their `DateTimeOffset` expiry) instead of raw JWT string

- `MyBuyingList.Application/ConfigureServices.cs`
  — register `IRefreshTokenService` → `RefreshTokenService` (scoped)
  — bind `RefreshTokenOptions` from configuration

### Infrastructure
- `MyBuyingList.Infrastructure/ConfigureServices.cs`
  — register `IRefreshTokenRepository` → `RefreshTokenRepository` (scoped)
  — register `RefreshTokenCleanupService` as `AddHostedService<RefreshTokenCleanupService>()`

### Web
- `MyBuyingList/Controllers/AuthController.cs`
  — `Authenticate`: update `ProducesResponseType` to `LoginResponse`, return `Ok(loginResponse)`
  — add `POST refresh` action: `[HttpPost("refresh")]`, accepts `RefreshRequest`, calls `IRefreshTokenService.RefreshAsync`, returns `LoginResponse`
  — apply `[EnableRateLimiting(AuthenticationRateLimiterPolicy.PolicyName)]` to `refresh` too

### Configuration
- `appsettings.json` — add:
  ```json
  "RefreshTokenOptions": {
    "ExpirationDays": 7,
    "RevokedTokenRetentionDays": 30
  }
  ```
- `appsettings.Development.json` — same section

---

## Implementation Steps (in order)

1. **Domain** — `RefreshToken` entity + `User` navigation + `FieldLengths` constant
2. **Application interfaces/DTOs** — `LoginResponse`, `RefreshRequest`, `IRefreshTokenRepository`, `RefreshTokenOptions`, `IRefreshTokenService`, validators
3. **Application services** — update `LoginService`; implement `RefreshTokenService` (with theft logging)
4. **Infrastructure** — `RefreshTokenConfiguration`, `RefreshTokenRepository`, `RefreshTokenCleanupService`, update `ConfigureServices`
5. **Run EF migration**
6. **Web** — update `AuthController`, update `ConfigureServices` registrations
7. **Configuration** — add `RefreshTokenOptions` to appsettings files
8. **Tests**
   - Unit: `RefreshTokenService` (valid rotation, expired token, revoked token, reuse detection + warning log assertion)
   - Unit: updated `LoginService` tests (now returns `LoginResponse` with expiry fields)
   - Unit: `RefreshTokenCleanupService` (verifies repository method called with correct cutoff date)
   - Integration: `POST /api/auth/refresh` happy path + error cases
   - Update existing `AuthController` integration tests for new JSON response shape

---

## API Contract

### `POST /api/auth` (updated response)
```json
// 200 OK
{
  "accessToken": "eyJ...",
  "accessTokenExpiresAt": "2026-04-05T14:32:00+00:00",
  "refreshToken": "base64url-random-token",
  "refreshTokenExpiresAt": "2026-04-12T14:32:00+00:00"
}
// 401 Unauthorized — unchanged
```

### `POST /api/auth/refresh` (new)
```json
// Request
{ "refreshToken": "base64url-random-token" }

// 200 OK
{
  "accessToken": "eyJ...",
  "accessTokenExpiresAt": "2026-04-05T14:33:00+00:00",
  "refreshToken": "base64url-new-random-token",
  "refreshTokenExpiresAt": "2026-04-12T14:33:00+00:00"
}
// 401 Unauthorized — token missing, expired, revoked, or reuse detected
```

---

## Security Notes

- Raw token is **never persisted**. Only its SHA-256 hex hash is stored.
- `GetByTokenHashAsync` looks up by hash — timing-safe via DB index lookup.
- Reuse of a revoked token triggers `RevokeAllForUserAsync` **and** a `LogWarning` with the userId so the operator can investigate in Seq.
  - **Known trade-off**: this is a nuclear revocation — all sessions across all devices are invalidated, including sessions that were not involved in the theft. This is intentional: when theft is detected, forcing a full re-authentication across all devices is the safest response. A comment must be left in `RefreshTokenService` explaining this decision.
- `IsRevoked` + `ExpiresAt` are both checked on every refresh.
- Rate limiting on `/refresh` prevents brute-forcing the token space.
- Cleanup service retains revoked tokens for `RevokedTokenRetentionDays` (default 30) to allow post-incident forensics before hard deletion.

# Dotnet WebAPI Template

This project aims to be a template for new WebAPI projects. It already sets up:

Tech stack:
* .NET 8
* Swagger
* Postgres with Entity Framework Core
* Docker and Docker Swarm
* xUnit, AutoFixture, NSubstitute and TestContainers
* FluentValidation
* BCrypt.Net

## Features implemented

### Architecture and DDD

This project adopts what many .NET influencers commonly refer to as "Clean Architecture" with Domain-Driven Design (DDD). The organizational structure draws inspiration from the accumulated knowledge of various projects developed over time.

The solution is split into four projects:

* **Domain** — entities, value objects, domain constants and domain exceptions.
* **Application** — use-case services, DTOs, validators, mappers and interfaces. No framework dependencies.
* **Infrastructure** — EF Core, JWT, repositories and seeders. Implements the interfaces defined in Application.
* **Web** — ASP.NET controllers, middlewares, filters and startup configuration.

### API Contracts and Documentation

This template comes pre-configured with Swagger UI for API documentation. It maps all possible status codes that may be returned from each endpoint, along with the required input payloads and the corresponding response objects. Swagger is served at the application root (`/`).

### Docker

This project comes pre-configured with Dockerfiles, aiming to deploy to production in Docker Swarm mode and develop locally with Docker Compose. See `docker-compose.yml`, `docker-compose.development.yml` and `docker-compose.production.yml`.

### Middlewares, Filters and Exception/Error Handling

**Request validation filter**

FluentValidation is used via the `RequestBodyValidationFilter` class to validate user input data (represented as DTOs). This filter was created to replace FluentValidation.AspNetCore automatic validation, since FluentValidation itself [does not recommend using it](https://docs.fluentvalidation.net/en/latest/aspnet.html). The filter adds async support for automatic validation.

Request buffering is also enabled globally so the filter can read the request body more than once.

**Exception handling middleware**

`ErrorHandlingMiddleware` sits at the top of the pipeline and handles all unhandled exceptions:

* `OperationCanceledException` — the request is aborted cleanly without an error response.
* Any exception implementing `IFormattedResponseException` — the middleware uses the exception's `StatusCode` and `ErrorResponse` to return a structured JSON error body.
* All other exceptions — a `500 Internal Server Error` is returned with no body (to avoid leaking internals).

The following custom exceptions are provided and all implement `IFormattedResponseException`:

| Exception | HTTP Status |
|---|---|
| `BadRequestException` | 400 |
| `AuthenticationException` | 401 |
| `AccountLockedException` | 401 |
| `ResourceNotFoundException` | 404 |
| `BusinessLogicException` | 409 |
| `DatabaseException` | 500 |
| `InternalServerErrorException` | 500 |

### Rate Limiting

Rate limiting is applied **only on the authentication endpoint** via `AuthenticationRateLimiterPolicy`, using the built-in ASP.NET Core rate limiter (`Microsoft.AspNetCore.RateLimiting`).

The policy uses a **fixed window** algorithm partitioned by client IP address. When the limit is exceeded the middleware returns `429 Too Many Requests`.

Configuration is driven by `CustomRateLimiterOptions`, read from `appsettings.json` under the `CustomRateLimiterOptions` key:

| Option | Description |
|---|---|
| `PermitLimit` | Number of requests allowed per window |
| `QueueLimit` | Number of requests queued when the limit is reached |
| `Window` | Window duration in seconds |

### Authentication and Authorization

**Authentication**

JWT Bearer authentication is used. The JWT setup lives under `Infrastructure/Authentication/JwtSetup`.

**Authorization**

Policy-based authorization is implemented with the following components:

* `PermissionRequirement` — wraps a single permission string.
* `HasPermissionAttribute` — decorates controller actions with the required permission name.
* `PermissionAuthorizationHandler` — validates that the JWT contains a claim matching the required permission.
* `PermissionAuthorizationPolicyProvider` — builds policies on demand so they don't all need to be registered at startup.

Request flow:

1. The controller action declares a required permission via `HasPermissionAttribute`.
2. `PermissionAuthorizationPolicyProvider` builds the corresponding policy.
3. `PermissionAuthorizationHandler` checks the JWT claims.
4. The handler marks the context as succeeded or failed.

Permissions are defined as constants in the Domain layer (`Policies` class) and are seeded into the database via migrations, which keeps code and database in sync.

### Account Lockout

Account lockout protects against brute-force attacks on the login endpoint. Failed attempts are tracked directly on the `User` entity (`FailedLoginAttempts`, `LockoutEnd`).

Once the configured number of failed attempts is reached the account is locked until `LockoutEnd` elapses. Subsequent login attempts — even with the correct password — return `401 Unauthorized` with a "temporarily locked" message via `AccountLockedException`.

Configuration is read from `appsettings.json` under the `LockoutOptions` key:

| Option | Default | Description |
|---|---|---|
| `MaxFailedAttempts` | `5` | Number of consecutive failures before locking |
| `LockoutDurationMinutes` | `15` | How long the account stays locked |

### Database and EFCore

This template uses Postgres alongside EFCore and comes configured with migrations. Migrations are **applied automatically on startup** before the application begins serving requests.

* Entity Framework is configured to use **eager loading** and **no-tracking** by default.
* Cancellation tokens are passed through every repository call.
* Passwords are stored using the **BCrypt** algorithm (hash + salt).

**Admin user seeder**

On every startup, `AdminUserSeeder` runs after migrations. It creates the admin user (with a fixed ID) if it does not exist yet, or updates the stored email and password hash if the credentials configured in `AdminSettings` have changed since the last run. This keeps the admin account in sync with the deployment configuration without requiring a manual migration.

Admin credentials are read from `AdminSettings` (`AdminSettings__Email` and `AdminSettings__Password` environment variables / Docker secrets).

### Logging

Log configuration is set in `appsettings.{ENVIRONMENT}.json`. See the [Microsoft logging documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-8.0#aspnet-core-and-ef-core-categories) for available categories and levels.

### Network

According to [Microsoft](https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-8.0&tabs=visual-studio%2Clinux-ubuntu), APIs should either not listen on HTTP or close the connection with status code 400. Therefore, this application is configured to listen on **HTTPS only**. This is enforced in three places: `launchSettings.json`, `Dockerfile` and Docker Compose files.

### Testing

There are three test projects:

* **`MyBuyingList.Application.Tests`** — unit tests for services, validators and mappers using xUnit, AutoFixture and NSubstitute.
* **`MyBuyingList.Infrastructure.Tests`** — unit tests for infrastructure concerns (e.g. JWT token generation).
* **`MyBuyingList.Web.Tests`** — integration tests for the full HTTP stack. Tests spin up a real Postgres instance via TestContainers (`postgres:14`) and use `WebApplicationFactory` to host the application in-process.

## How to

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* [Docker](https://www.docker.com/)

### Run locally

1. Clone the repository.
2. Populate the secret files under `MyBuyingList/local/secrets/` (see the `.csproj` for the expected file names: `JwtSettings__SigningKey`, `ConnectionStrings__DefaultConnection`, `AdminSettings__Email`, `AdminSettings__Password`).
3. Start Postgres (and pgAdmin) using Docker Compose. There are two ways:
   - **Via the `.dcproj` file** — open `docker-compose.dcproj` in Visual Studio or Rider, which will manage the compose lifecycle for you.
   - **Via the CLI:**
     ```bash
     docker compose -f docker-compose.development.yml up -d
     ```
   > **Before running**, open `docker-compose.development.yml` and review the volume mappings under the `api` service. The file is pre-configured for **macOS** (`~/.aspnet/dev-certs/https`). If you are on **Windows**, comment out the macOS lines and uncomment the Windows lines as shown in the comments inside the file.
4. Run the application. Choose one of two approaches:
   - **API inside Docker** (recommended — API and DB share the same network): Docker Compose already handles this; no further steps needed.
   - **API outside Docker** (`dotnet run`): open `MyBuyingList/appsettings.Development.json`, copy the connection string commented as `// If running locally (not container)`, and paste it as the content of `MyBuyingList/local/secrets/ConnectionStrings__DefaultConnection`. Then run:
     ```bash
     dotnet run --project MyBuyingList
     ```
   Migrations and the admin user seed are applied automatically on startup.
5. Open `https://localhost:{port}` to access the Swagger UI.

### Run in production

The application is designed to run in **Docker Swarm** mode. The production compose setup merges `docker-compose.yml` (base configuration) and `docker-compose.production.yml` (image, secrets, TLS volumes). Secrets are stored as Docker secrets and never baked into the image.

#### 1. Initialize Docker Swarm

```bash
docker swarm init
```

#### 2. Create Docker secrets

```bash
# Connection string to your database
echo '<your-connection-string>' >> conn_string.txt
docker secret create ConnectionStrings__DefaultConnection conn_string.txt
rm conn_string.txt

# JWT signing key (use a long, random string)
echo '<your-jwt-signing-key>' >> jwt.txt
docker secret create JwtSettings__SigningKey jwt.txt
rm jwt.txt

# Admin account credentials
echo '<admin@yourdomain.com>' >> admin_email.txt
docker secret create AdminSettings__Email admin_email.txt
rm admin_email.txt

echo '<your-admin-password>' >> admin_pwd.txt
docker secret create AdminSettings__Password admin_pwd.txt
rm admin_pwd.txt
```

#### 3. Configure TLS

The application expects a TLS certificate at `/etc/letsencrypt/live/<your-domain>/` on the host (mapped as a volume in `docker-compose.production.yml`). Replace `<your-domain>` in `docker-compose.production.yml` with your actual domain name.

**Bootstrap with a self-signed certificate** (will be replaced by a real cert in the next step):

```bash
sudo mkdir -p /etc/letsencrypt/live/<your-domain>

sudo openssl req -x509 -newkey rsa:4096 \
  -keyout /etc/letsencrypt/live/<your-domain>/privkey.pem \
  -out /etc/letsencrypt/live/<your-domain>/fullchain.pem \
  -days 365 -nodes \
  -subj "/CN=<your-domain>"

sudo chmod 600 /etc/letsencrypt/live/<your-domain>/privkey.pem
sudo chmod 644 /etc/letsencrypt/live/<your-domain>/fullchain.pem
```

> **Note:** While using a self-signed certificate, the healthcheck in `docker-compose.production.yml` uses `curl -k` to skip certificate validation. Once a valid certificate is installed, remove the `-k` flag from the healthcheck command.

**Replace with a Let's Encrypt certificate** once the domain is pointing to your VPS ([Certbot instructions](https://certbot.eff.org/instructions?ws=other&os=snap)):

```bash
sudo apt update && sudo apt install snapd -y
sudo snap install --classic certbot
sudo ln -s /snap/bin/certbot /usr/local/bin/certbot

sudo certbot certonly --standalone -d <your-domain>
```

#### 4. Deploy the stack

```bash
cd ~/app
docker stack deploy \
  -c docker-compose.yml \
  -c docker-compose.production.yml \
  myapp
```

### Run tests

```bash
dotnet test
```

Integration tests require Docker to be running (TestContainers will start a Postgres container automatically).

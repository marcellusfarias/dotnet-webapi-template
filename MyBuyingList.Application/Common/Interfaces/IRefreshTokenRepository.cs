using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string hash, CancellationToken cancellationToken);
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken);
    Task RevokeAsync(RefreshToken token, CancellationToken cancellationToken);
    Task RevokeAllForUserAsync(int userId, CancellationToken cancellationToken);
    Task DeleteExpiredAndRevokedAsync(DateTimeOffset revokedBefore, CancellationToken cancellationToken);
}

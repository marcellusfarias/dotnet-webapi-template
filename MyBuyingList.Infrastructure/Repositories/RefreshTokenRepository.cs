using Microsoft.EntityFrameworkCore;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string hash, CancellationToken cancellationToken)
    {
        try
        {
            return await _context.Set<RefreshToken>()
                .Include(t => t.User)
                .SingleOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex) { throw new DatabaseException(ex); }
    }

    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        try
        {
            _context.Set<RefreshToken>().Add(token);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex) { throw new DatabaseException(ex); }
    }

    public async Task RevokeAndAddAsync(RefreshToken tokenToRevoke, RefreshToken newToken, CancellationToken cancellationToken)
    {
        try
        {
            tokenToRevoke.IsRevoked = true;
            _context.Entry(tokenToRevoke).State = EntityState.Modified;
            _context.Set<RefreshToken>().Add(newToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex) { throw new DatabaseException(ex); }
    }

    public async Task RevokeAllForUserAsync(int userId, CancellationToken cancellationToken)
    {
        try
        {
            await _context.Set<RefreshToken>()
                .Where(t => t.UserId == userId && !t.IsRevoked)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsRevoked, true), cancellationToken);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex) { throw new DatabaseException(ex); }
    }

    public async Task DeleteExpiredAndRevokedAsync(DateTimeOffset revokedBefore, CancellationToken cancellationToken)
    {
        try
        {
            await _context.Set<RefreshToken>()
                .Where(t => t.ExpiresAt < DateTimeOffset.UtcNow || (t.IsRevoked && t.CreatedAt < revokedBefore))
                .ExecuteDeleteAsync(cancellationToken);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex) { throw new DatabaseException(ex); }
    }
}

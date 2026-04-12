using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Common.Options;

namespace MyBuyingList.Infrastructure.BackgroundServices;

public class RefreshTokenCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RefreshTokenCleanupService> _logger;
    private readonly int _revokedTokenRetentionDays;

    public RefreshTokenCleanupService(
        IServiceScopeFactory scopeFactory,
        IOptions<RefreshTokenOptions> options,
        ILogger<RefreshTokenCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _revokedTokenRetentionDays = options.Value.RevokedTokenRetentionDays;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var revokedBefore = DateTimeOffset.UtcNow.AddDays(-_revokedTokenRetentionDays);
                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();
                await repository.DeleteExpiredAndRevokedAsync(revokedBefore, stoppingToken);
                _logger.LogInformation("Refresh token cleanup completed.");
                
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Refresh token cleanup canceled.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Refresh token cleanup failed.");
            }
        }
    }
}


namespace MyBuyingList.Application.Common.Interfaces;

public interface IJwtProvider
{
    Task<string> GenerateTokenAsync(int userId, CancellationToken cancellationToken);
}

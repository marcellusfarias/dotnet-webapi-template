using MyBuyingList.Application.Features.Login.DTOs;

namespace MyBuyingList.Application.Features.Login.Services;

public interface ILoginService
{
    Task<string> AuthenticateAndReturnJwtTokenAsync(LoginDto loginDto, CancellationToken cancellationToken);
}

using Microsoft.AspNetCore.Mvc;
using MyBuyingList.Application.Common.Interfaces.Services;

namespace MyBuyingList.Web.Controllers;
public class AuthController : ApiControllerBase
{
    private ILoginService _loginService;
    public AuthController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost]
    public async Task<IActionResult> Authenticate(string username, string password)
    {
        var jwtToken = await _loginService.AuthenticateAndReturnJwtTokenAsync(username, password);
        return string.IsNullOrEmpty(jwtToken) ? Unauthorized() : Ok(jwtToken);
    }
}

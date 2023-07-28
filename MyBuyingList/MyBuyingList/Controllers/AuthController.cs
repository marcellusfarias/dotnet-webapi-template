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
    public IActionResult Authenticate(string username, string password)
    {
        var jwtToken = _loginService.AuthenticateAndReturnJwtToken(username, password);
        return string.IsNullOrEmpty(jwtToken) ? Unauthorized() : Ok(jwtToken);
    }
}

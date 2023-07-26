using Microsoft.AspNetCore.Mvc;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Infrastructure.Authentication;

namespace MyBuyingList.Web.Controllers;
public class AuthController : ApiControllerBase
{
    private IJwtProvider _jwtProvider;
    public AuthController(IJwtProvider jwtProvider)
    {
        _jwtProvider = jwtProvider;
    }

    [HttpPost]
    public IActionResult Authenticate(string username, string password)
    {
        if (string.IsNullOrEmpty(username)|| string.IsNullOrEmpty(password))
            return BadRequest("Invalid username or password.");

        var jwtToken = _jwtProvider.AuthenticateAndReturnToken(username, password);

        return Ok(jwtToken);
    }
}

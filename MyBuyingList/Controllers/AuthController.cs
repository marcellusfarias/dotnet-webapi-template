using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyBuyingList.Application.Features.Login.DTOs;
using MyBuyingList.Application.Features.Login.Services;

namespace MyBuyingList.Web.Controllers;
public class AuthController : ApiControllerBase
{
    private readonly ILoginService _loginService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILoginService loginService, ILogger<AuthController> logger)
    {
        _loginService = loginService;
        _logger = logger;
    }

    [EnableRateLimiting("Authentication")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [HttpPost, Produces("text/plain")]
    public async Task<IActionResult> Authenticate(
        [FromBody] LoginDto loginDto, 
        CancellationToken token)
    {
        var guid = new Guid();
        _logger.LogInformation($"{guid} - Authenticate user {loginDto.Username}");

        var jwtToken = await _loginService.AuthenticateAndReturnJwtTokenAsync(loginDto, token);

        if(string.IsNullOrEmpty(jwtToken))
        {
            _logger.LogInformation($"{guid} - Unauthorized {loginDto.Username}");
            return Unauthorized();
        }
        else
        {
            _logger.LogInformation($"{guid} - Authenticated {loginDto.Username}");
            return Ok(jwtToken);
        }
    }
}

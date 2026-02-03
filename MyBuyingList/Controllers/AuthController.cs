using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyBuyingList.Application.Features.Login.DTOs;
using MyBuyingList.Application.Features.Login.Services;
using MyBuyingList.Web.Middlewares.RateLimiting;

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

    [EnableRateLimiting(AuthenticationRateLimiterPolicy.PolicyName)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [HttpPost, Produces("text/plain")]
    public async Task<IActionResult> Authenticate(
        [FromBody] LoginRequest loginDto, 
        CancellationToken token)
    {
        Guid guid = Guid.NewGuid();
        _logger.LogInformation("{Guid} - Authenticate user {LoginRequestUsername}", guid, loginDto.Username);

        var jwtToken = await _loginService.AuthenticateAndReturnJwtTokenAsync(loginDto, token);

        if(string.IsNullOrEmpty(jwtToken))
        {
            _logger.LogInformation("{Guid} - Unauthorized {LoginRequestUsername}", guid, loginDto.Username);
            return Unauthorized();
        }

        _logger.LogInformation("{Guid} - Authenticated {LoginRequestUsername}", guid, loginDto.Username);
        return Ok(jwtToken);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Features.Login.DTOs;
using MyBuyingList.Application.Features.Login.Services;
using MyBuyingList.Web.Middlewares.RateLimiting;

namespace MyBuyingList.Web.Controllers;

public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [EnableRateLimiting(AuthenticationRateLimiterPolicy.PolicyName)]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<IActionResult> Authenticate(
        [FromBody] LoginRequest loginDto,
        CancellationToken token)
    {
        var safeUsername = SanitizeForLog(loginDto.Username);
        _logger.LogInformation("Authenticate user {LoginRequestUsername}", safeUsername);

        var loginResponse = await _authService.AuthenticateAsync(loginDto, token);

        _logger.LogInformation("Authenticated {LoginRequestUsername}", safeUsername);
        return Ok(loginResponse);
    }

    [EnableRateLimiting(AuthenticationRateLimiterPolicy.PolicyName)]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshRequest refreshRequest,
        CancellationToken token)
    {
        var loginResponse = await _authService.RefreshAsync(refreshRequest, token);
        return Ok(loginResponse);
    }
}

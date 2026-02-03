using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Application.Features.Users.Services;
using MyBuyingList.Domain.Constants;
using MyBuyingList.Web.Middlewares.Authorization;

namespace MyBuyingList.Web.Controllers;

[Authorize]
public class UserController : ApiControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;
    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _userService = userService;
        _logger = logger;
    }

    [HasPermission(Policies.UserGetAll)]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("~/api/users/")]
    public async Task<IActionResult> GetAllUsers(
        CancellationToken token,
        [FromQuery] int page = 1)
    {
        _logger.LogInformation("Starting request. Page = {Page}", page);

        var users = await _userService.GetAllUsersAsync(page, token);
        return Ok(users);
    }

    [HasPermission(Policies.UserGet)]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(
        [FromRoute] int id,
        CancellationToken token)
    {
        _logger.LogInformation("Starting request. Id = {Id}", id);

        var user = await _userService.GetUserAsync(id, token);
        return Ok(user);
    }

    [HasPermission(Policies.UserCreate)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateUserRequest createUserDto,
        CancellationToken token)
    {
        var guid = Guid.NewGuid();
        _logger.LogInformation("{Guid} - Create user {UserName} with email {Email}", guid, createUserDto.UserName, createUserDto.Email);

        var userId = await _userService.CreateAsync(createUserDto, token);

        _logger.LogInformation("{Guid} - Created with id {UserId}", guid, userId);

        return new ObjectResult(userId) { StatusCode = StatusCodes.Status201Created };
    }

    [HasPermission(Policies.UserUpdate)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id}/password")]
    public async Task<IActionResult> ChangePassword(
        [FromRoute] int id,
        ChangePasswordRequest updateUserPasswordDto,
        CancellationToken token)
    {
        var guid = Guid.NewGuid();
        _logger.LogInformation("{Guid} - Change password from user {Id}", guid, id);

        await _userService.ChangeUserPasswordAsync(id, updateUserPasswordDto.OldPassword, updateUserPasswordDto.NewPassword, token);

        _logger.LogInformation("{Guid} - Password from user {Id} updated", guid, id);

        return NoContent();
    }

    [HasPermission(Policies.UserDelete)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        [FromRoute] int id,
        CancellationToken token)
    {
        var guid = Guid.NewGuid();
        _logger.LogInformation("{Guid} - Delete user {Id}", guid, id);

        await _userService.DeleteAsync(id, token);

        _logger.LogInformation("{Guid} - User {Id} deleted", guid, id);

        return NoContent();
    }

}

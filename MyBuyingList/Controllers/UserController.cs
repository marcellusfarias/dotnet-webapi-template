﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HasPermission(Policies.GetAllUsers)]
    [ProducesResponseType(typeof(List<GetUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
    [HttpGet("~/api/users/")]
    public async Task<IActionResult> GetAllUsers(
        CancellationToken token,
        [FromQuery] int page = 1)
    {
        _logger.LogInformation($"Starting request. Page = {page}");

        var users = await _userService.GetAllUsersAsync(page, token);
        return Ok(users);
    }

    [HasPermission(Policies.GetUser)]
    [ProducesResponseType(typeof(List<GetUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
    [HttpGet("~/api/user/{id}")]
    public async Task<IActionResult> GetUserById(
        [FromRoute] int id,
        CancellationToken token)
    {
        _logger.LogInformation($"Starting request. Id = {id}");

        var user = await _userService.GetUserAsync((int)id, token);
        return Ok(new List<GetUserDto>() { user });
    }

    // TODO: change return to GetUserDto
    [HasPermission(Policies.CreateUser)]
    [ProducesResponseType(typeof(CreateUserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateUserDto createUserDto,
        CancellationToken token)
    {
        await _userService.CreateAsync(createUserDto, token);
        return new ObjectResult(createUserDto) { StatusCode = StatusCodes.Status201Created };
    }

    [HasPermission(Policies.UpdateUser)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id}/password")]
    public async Task<IActionResult> ChangePassword(
        [FromRoute] int id,
        UpdateUserPasswordDto updateUserPasswordDto,
        CancellationToken token)
    {
        await _userService.ChangeUserPasswordAsync(id, updateUserPasswordDto.OldPassword, updateUserPasswordDto.NewPassword, token);
        return NoContent();
    }

    [HasPermission(Policies.DeleteUser)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        [FromRoute] int id,
        CancellationToken token)
    {
        await _userService.DeleteAsync(id, token);
        return NoContent();
    }

}

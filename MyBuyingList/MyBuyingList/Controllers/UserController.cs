using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.Contracts.User;
using MyBuyingList.Application.DTOs.UserDtos;
using MyBuyingList.Infrastructure.Auth.AuthorizationHandlers;
using MyBuyingList.Infrastructure.Auth.Constants;

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

    //TODO: pagination
    [HasPermission(Policies.GetAllUsers)]
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _userService.GetAllUsers();
        return Ok(users);
    }

    [HasPermission(Policies.CreateUser)]
    [HttpPost]
    public IActionResult Create(CreateUserDto createUserDto)
    {
        _userService.Create(createUserDto);
        return new ObjectResult(createUserDto) { StatusCode = StatusCodes.Status201Created };
    }

    [HasPermission(Policies.UpdateUser)]
    [HttpPut("ChangeActiveStatus")]
    public IActionResult ChangeActiveStatus(int userId, bool activeStatus)
    {
        _userService.ChangeActiveStatus(userId, activeStatus);
        return NoContent();
    }

    [HasPermission(Policies.UpdateUser)]
    [HttpPut("ChangePassword")]
    public IActionResult ChangePassword(UpdateUserPasswordDto updateUserPasswordDto)
    {
        _userService.ChangeUserPassword(updateUserPasswordDto.Id, updateUserPasswordDto.OldPassword, updateUserPasswordDto.NewPassword);
        return NoContent();
    }

    [HasPermission(Policies.DeleteUser)]
    [HttpDelete]
    public IActionResult Delete(int id)
    {
        _userService.Delete(id); //throws ResourceNotFoundException
        return NoContent();
    }

}

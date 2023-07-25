using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.DTOs;
using MyBuyingList.Infrastructure.Authentication;

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
    [HttpGet]
    public IActionResult Get()
    {
        var users = _userService.List();
        return Ok(users);
    }

    [HasPermission(Policies.CreateUser)]
    [HttpPost]
    public IActionResult Create(UserDto userDto)
    {
        _userService.Create(userDto);
        return new ObjectResult(userDto) { StatusCode = StatusCodes.Status201Created };
    }

    [HasPermission(Policies.UpdateUser)]
    [HttpPut]
    public IActionResult Update(UserDto userDto)
    {
        _userService.Create(userDto);
        return new ObjectResult(userDto) { StatusCode = StatusCodes.Status201Created };
    }

}

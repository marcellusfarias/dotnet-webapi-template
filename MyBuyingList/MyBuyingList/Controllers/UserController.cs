using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MyBuyingList.Application.Common.Errors;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.DTOs;

namespace MyBuyingList.Web.Controllers
{
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var users = _userService.List();
            return Ok(users);
        }

        [HttpPost]
        public IActionResult Create(UserDto userDto)
        {
            _userService.Create(userDto);
            return Created("", userDto);
        }
    }
}

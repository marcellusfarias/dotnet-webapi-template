using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        public ActionResult<UserDto> Get()
        {
            var list = _userService.List();
            return Ok(list);
        }

        [HttpPost]
        public IActionResult Create(UserDto userDto)
        {
            return _userService
                .Create(userDto)
                .Match<IActionResult>(
                m => Ok(userDto.Id),
                err => BadRequest(err.Message)
                );
        }
    }
}

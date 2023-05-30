using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            _userService.Create(userDto);
            return Ok(userDto.Id);
        }
    }
}

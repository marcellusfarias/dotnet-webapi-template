using Microsoft.AspNetCore.Mvc;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Infrastructure.Authentication;

namespace MyBuyingList.Web.Controllers
{
    public class LoginController : ApiControllerBase
    {
        private ICustomAuthenticationService _authenticationService;
        public LoginController(ICustomAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username)|| string.IsNullOrEmpty(password))
                return BadRequest("Invalid username or password.");

            var jwtToken = _authenticationService.AuthenticateAndReturnToken(username, password);

            return Ok(jwtToken);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Infrastructure.Authentication;

namespace MyBuyingList.Web.Controllers
{
    public class LoginController : ApiControllerBase
    {
        //inject this with some interface. Should not call anything from infrastructure, but rather application.
        private IJwtProvider _jwtProvider;
        public LoginController(IJwtProvider jwtProvider)
        {
            _jwtProvider = jwtProvider;
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            //compare hashed password with the one in database

            //Generate and return if valid.
            return Ok(_jwtProvider.Generate(email));
        }
    }
}

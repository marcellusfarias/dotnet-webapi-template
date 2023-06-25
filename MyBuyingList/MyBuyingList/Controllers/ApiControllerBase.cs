using Microsoft.AspNetCore.Mvc;

namespace MyBuyingList.Web.Controllers;

[ApiController]
//[ApiExceptionFilter]
[Route("api/[controller]")]
public class ApiControllerBase : ControllerBase { }

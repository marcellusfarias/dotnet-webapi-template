using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace MyBuyingList.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces("application/json")]
public class ApiControllerBase : ControllerBase { }

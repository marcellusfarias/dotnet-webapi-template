using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace MyBuyingList.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class ApiControllerBase : ControllerBase { }

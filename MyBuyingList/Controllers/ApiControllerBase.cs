using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace MyBuyingList.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class ApiControllerBase : ControllerBase
{
    protected static string SanitizeForLog(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var sanitized = input.Replace("\r", string.Empty).Replace("\n", string.Empty);
        return sanitized.Length > 200 ? sanitized[..200] : sanitized;
    }
}

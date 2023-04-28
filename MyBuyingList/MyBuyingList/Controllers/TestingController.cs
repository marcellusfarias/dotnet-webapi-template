using Microsoft.AspNetCore.Mvc;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Controllers;

namespace MyBuyingList.Web.Controllers;

[Route("api/[controller]")]
public class TestingController : Controller
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<TestingController> _logger;

    public TestingController(ILogger<TestingController> logger, IApplicationDbContext context)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        _logger.LogInformation("I'm here");
        var users = _context.Users.ToList();
        //var records 
        return Ok(users);
    }
}

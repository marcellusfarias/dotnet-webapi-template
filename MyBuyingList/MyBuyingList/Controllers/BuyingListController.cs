using Microsoft.AspNetCore.Mvc;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.DTOs;
using MyBuyingList.Infrastructure.Auth.AuthorizationHandlers;
using MyBuyingList.Infrastructure.Auth.Constants;

namespace MyBuyingList.Web.Controllers;

public class BuyingListController : ApiControllerBase
{
    private readonly IBuyingListService _buyingListService;
    private readonly ILogger<BuyingListController> _logger;
    public BuyingListController(ILogger<BuyingListController> logger, IBuyingListService buyingListService)
    {
        _buyingListService = buyingListService;
        _logger = logger;
    }

    //TODO: pagination
    //[HasPermission(Policies.BuyingListGet)]
    //[HttpGet]
    //public IActionResult Get(int buyingListId)
    //{
    //    var users = _buyingListService.GetById();
    //    return Ok(users);
    //}

    [HasPermission(Policies.BuyingListCreate)]
    [HttpPost]
    public IActionResult Create(BuyingListDto buyingListDto)
    {
        _buyingListService.Create(buyingListDto);
        return new ObjectResult(buyingListDto) { StatusCode = StatusCodes.Status201Created };
    }

    [HasPermission(Policies.BuyingListUpdate)]
    [HttpPut]
    public IActionResult Update(BuyingListDto buyingListDto)
    {
        _buyingListService.Update(buyingListDto);
        return new ObjectResult(buyingListDto) { StatusCode = StatusCodes.Status201Created };
    }

    [HasPermission(Policies.BuyingListDelete)]
    [HttpDelete]
    public IActionResult Delete(BuyingListDto buyingListDto)
    {
        //return 204 if no content?
        _buyingListService.Delete(buyingListDto);
        return Ok();
    }
}

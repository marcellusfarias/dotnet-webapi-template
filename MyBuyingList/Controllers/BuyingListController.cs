using Microsoft.AspNetCore.Mvc;
using MyBuyingList.Application.Features.BuyingLists.DTOs;
using MyBuyingList.Application.Features.BuyingLists.Services;
using MyBuyingList.Domain.Constants;
using MyBuyingList.Web.Middlewares.Authorization;
using NuGet.Protocol;
using System.IdentityModel.Tokens.Jwt;

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
    [HasPermission(Policies.BuyingListGet)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<IActionResult> Get(int buyingListId, CancellationToken token)
    {
        var buyingListDto = await _buyingListService.GetByIdAsync(buyingListId, token);
        return Ok(buyingListDto);
    }

    [HasPermission(Policies.BuyingListCreate)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateBuyingListDto buyingListDto, CancellationToken token)
    {
        if (!Int32.TryParse(HttpContext.User.FindFirst(JwtRegisteredClaimNames.NameId)?.Value, out int currentUserId))
        {
            _logger.LogError($"Can't get user id for user {HttpContext.User.ToJson()}");
            throw new Exception("Unexpected error. Can't get user id.");
        }            

        var id = await _buyingListService.CreateAsync(buyingListDto, currentUserId, token);
        return new ObjectResult(id) { StatusCode = StatusCodes.Status201Created };
    }

    [HasPermission(Policies.BuyingListUpdate)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut("ChangeName")]
    public async Task<IActionResult> UpdateChangeName(UpdateBuyingListNameDto buyingListDto, CancellationToken token)
    {
        await _buyingListService.ChangeNameAsync(buyingListDto, token);
        return NoContent();
    }

    [HasPermission(Policies.BuyingListDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete]
    public async Task<IActionResult> Delete(int buyingListId, CancellationToken token)
    {
        await _buyingListService.DeleteAsync(buyingListId, token);
        return NoContent();
    }
}

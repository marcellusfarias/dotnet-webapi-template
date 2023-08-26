﻿using Microsoft.AspNetCore.Mvc;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.Contracts.BuyingListDtos;
using MyBuyingList.Infrastructure.Auth.AuthorizationHandlers;
using MyBuyingList.Infrastructure.Auth.Constants;
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
    public IActionResult Get(int buyingListId)
    {
        var buyingListDto = _buyingListService.GetById(buyingListId);
        return Ok(buyingListDto);
    }

    [HasPermission(Policies.BuyingListCreate)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public IActionResult Create(CreateBuyingListDto buyingListDto)
    {
        if (!Int32.TryParse(HttpContext.User.FindFirst(JwtRegisteredClaimNames.NameId)?.Value, out int currentUserId))
        {
            _logger.LogError($"Can't get user id for user {HttpContext.User.ToJson()}");
            throw new Exception("Unexpected error. Can't get user id.");
        }            

        var id = _buyingListService.Create(buyingListDto, currentUserId);
        return new ObjectResult(id) { StatusCode = StatusCodes.Status201Created };
    }

    [HasPermission(Policies.BuyingListUpdate)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut("ChangeName")]
    public IActionResult UpdateChangeName(UpdateBuyingListNameDto buyingListDto)
    {
        _buyingListService.ChangeName(buyingListDto);
        return NoContent();
    }

    [HasPermission(Policies.BuyingListDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete]
    public IActionResult Delete(int buyingListId)
    {
        _buyingListService.Delete(buyingListId);
        return NoContent();
    }
}

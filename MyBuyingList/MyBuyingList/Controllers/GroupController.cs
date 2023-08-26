using Microsoft.AspNetCore.Mvc;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.Contracts.GroupDtos;
using MyBuyingList.Infrastructure.Auth.AuthorizationHandlers;
using MyBuyingList.Infrastructure.Auth.Constants;
using NuGet.Protocol;
using System.IdentityModel.Tokens.Jwt;

namespace MyBuyingList.Web.Controllers;

public class GroupController : ApiControllerBase
{
    private readonly IGroupService _groupService;
    private readonly ILogger<GroupController> _logger;
    public GroupController(IGroupService groupService, ILogger<GroupController> logger)
    {
        _groupService = groupService;
        _logger = logger;
    }

    //TODO: pagination
    [HasPermission(Policies.GroupGet)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public IActionResult Get(int groupId)
    {
        var buyingListDto = _groupService.GetById(groupId);
        return Ok(buyingListDto);
    }

    [HasPermission(Policies.GroupCreate)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public IActionResult Create(CreateGroupDto groupDto)
    {
        if (!Int32.TryParse(HttpContext.User.FindFirst(JwtRegisteredClaimNames.NameId)?.Value, out int currentUserId))
        {
            _logger.LogError($"Can't get user id for user {HttpContext.User.ToJson()}");
            throw new Exception("Unexpected error. Can't get user id.");
        }

        var id = _groupService.Create(groupDto, currentUserId);
        return new ObjectResult(id) { StatusCode = StatusCodes.Status201Created };
    }

    [HasPermission(Policies.GroupUpdate)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut("ChangeName")]
    public IActionResult UpdateChangeName(UpdateGroupNameDto groupDto)
    {
        _groupService.ChangeName(groupDto);
        return NoContent();
    }

    [HasPermission(Policies.BuyingListDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete]
    public IActionResult Delete(int groupId)
    {
        _groupService.Delete(groupId);
        return NoContent();
    }
}

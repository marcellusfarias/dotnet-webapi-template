using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Features.Groups.DTOs;
using MyBuyingList.Application.Features.Groups.Mappers;

namespace MyBuyingList.Application.Features.Groups.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;

    public GroupService(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<GetGroupDto?> GetByIdAsync(int id, CancellationToken token)
    {
        var group = await _groupRepository.GetAsync(id, token);

        if (group is null)
            throw new ResourceNotFoundException();

        return group.ToGetGroupDto();
    }

    //TODO: perform validations
    public async Task<int> CreateAsync(CreateGroupDto groupDto, int currentUserId, CancellationToken token)
    {
        var buyingList = groupDto.ToGroup(currentUserId, DateTime.Now);

        return await _groupRepository.AddAsync(buyingList, token);
    }

    //TODO: perform validations
    public async Task ChangeNameAsync(UpdateGroupNameDto dto, CancellationToken token)
    {
        var buyingList = await _groupRepository.GetAsync(dto.Id, token);
        if (buyingList is null)
            throw new ResourceNotFoundException();

        buyingList.GroupName = dto.GroupName;

        await _groupRepository.EditAsync(buyingList, token);
    }

    public async Task DeleteAsync(int groupId, CancellationToken token)
    {
        var group = await _groupRepository.GetAsync(groupId, token);

        if (group is null)
            throw new ResourceNotFoundException();

        if (group.BuyingLists.Count > 0)
            throw new BusinessLogicException($"Can't delete group {groupId}. There are buying lists associated with it.");

        await _groupRepository.DeleteAsync(group, token);
    }
}

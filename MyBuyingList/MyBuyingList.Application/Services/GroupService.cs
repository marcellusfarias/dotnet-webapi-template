using AutoMapper;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.Contracts.GroupDtos;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IMapper _mapper;

    public GroupService(IGroupRepository groupRepository, IMapper mapper)
    {
        _groupRepository = groupRepository;
        _mapper = mapper;
    }
    public async Task<GetGroupDto?> GetByIdAsync(int id, CancellationToken token)
    {
        var group = await _groupRepository.GetAsync(id, token);
        return group == null
            ? throw new ResourceNotFoundException()
            : _mapper.Map<GetGroupDto>(group);
    }
    
    public async Task<int> CreateAsync(CreateGroupDto groupDto, int currentUserId, CancellationToken token)
    {
        var buyingList = _mapper.Map<Group>(groupDto);
        buyingList.CreatedBy = currentUserId;

        return await _groupRepository.AddAsync(buyingList, token);
    }

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

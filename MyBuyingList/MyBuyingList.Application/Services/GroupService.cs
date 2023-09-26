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
    public async Task<GetGroupDto?> GetByIdAsync(int id)
    {
        var group = await _groupRepository.GetAsync(id);
        return group == null
            ? throw new ResourceNotFoundException()
            : _mapper.Map<GetGroupDto>(group);
    }
    
    public async Task<int> CreateAsync(CreateGroupDto groupDto, int currentUserId)
    {
        var buyingList = _mapper.Map<Group>(groupDto);
        buyingList.CreatedBy = currentUserId;

        return await _groupRepository.AddAsync(buyingList);
    }

    public async Task ChangeNameAsync(UpdateGroupNameDto dto)
    {
        var buyingList = await _groupRepository.GetAsync(dto.Id);
        if (buyingList is null)
            throw new ResourceNotFoundException();

        buyingList.GroupName = dto.GroupName;

        await _groupRepository.EditAsync(buyingList);
    }

    public async Task DeleteAsync(int groupId)
    {
        var group = await _groupRepository.GetAsync(groupId);

        if (group is null)
            throw new ResourceNotFoundException();

        if (group.BuyingLists.Count > 0)
            throw new BusinessLogicException($"Can't delete group {groupId}. There are buying lists associated with it.");

        await _groupRepository.DeleteAsync(group);
    }
}

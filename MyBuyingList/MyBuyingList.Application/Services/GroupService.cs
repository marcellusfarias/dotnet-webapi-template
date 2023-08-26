using AutoMapper;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.Contracts.BuyingListDtos;
using MyBuyingList.Application.Contracts.GroupDtos;
using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public GetGroupDto? GetById(int id)
    {
        var group = _groupRepository.Get(id);
        return group == null
            ? throw new ResourceNotFoundException()
            : _mapper.Map<GetGroupDto>(group);
    }
    public int Create(CreateGroupDto groupDto, int currentUserId)
    {
        var buyingList = _mapper.Map<Group>(groupDto);
        buyingList.CreatedBy = currentUserId;

        return _groupRepository.Add(buyingList);
    }

    public void ChangeName(UpdateGroupNameDto dto)
    {
        var buyingList = _groupRepository.Get(dto.Id);
        if (buyingList is null)
            throw new ResourceNotFoundException();

        buyingList.GroupName = dto.GroupName;

        _groupRepository.Edit(buyingList);
    }

    public void Delete(int groupId)
    {
        var group = _groupRepository.Get(groupId);

        if (group is null)
            throw new ResourceNotFoundException();

        if (group.BuyingLists.Count > 0)
            throw new BusinessLogicException();

        _groupRepository.Delete(group);
    }    
}

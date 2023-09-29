using MyBuyingList.Application.Features.Groups.DTOs;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Features.Groups.Mappers;

public static class GroupMapperDtoToDomain
{
    public static Group ToGroup(this CreateGroupDto dto, int userId, DateTime createdAt)
    {
        return new Group
        {
            Id = 0,
            GroupName = dto.GroupName,
            CreatedBy = userId,
            CreatedAt = createdAt,
        };
    }
}

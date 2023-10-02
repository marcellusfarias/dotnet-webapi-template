using MyBuyingList.Application.Features.BuyingLists.DTOs;
using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Features.BuyingLists.Mappers;

internal static class BuyingListMapperDtoToDomain
{
    internal static BuyingList ToBuyingList(this CreateBuyingListDto dto, int userId, DateTime createdAt)
    {
        return new BuyingList
        {
            Name = dto.Name,
            GroupId = dto.GroupId,
            CreatedBy = userId,
            CreatedAt = createdAt,
        };
    }
}

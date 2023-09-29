using MyBuyingList.Application.Features.BuyingLists.DTOs;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Features.BuyingLists.Mappers;

internal static class BuyingListMapperDomainToDto
{
    internal static GetBuyingListDto ToGetBuyingListDto(this BuyingList buyingList)
    {
        return new GetBuyingListDto
        {
            Name = buyingList.Name,
            GroupId = buyingList.GroupId,
            CreatedBy = buyingList.CreatedBy,
            CreatedAt = buyingList.CreatedAt,
        };
    }
}
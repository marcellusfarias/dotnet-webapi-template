using AutoMapper;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Application.Common.Mappings;

namespace MyBuyingList.Application.Contracts.BuyingListDtos;

public class GetBuyingListDto
{
    public required string Name { get; set; }
    public required int GroupId { get; set; }
    public required int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

[AutoMapperMapping]
public class CreateUserDtoMapping
{
    public void ConfigureMappings(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<GetBuyingListDto, BuyingList>();
    }
}
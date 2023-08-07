using AutoMapper;
using MyBuyingList.Application.DTOs;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Common.Mappings;

public class DefaultProfile : Profile
{
    public DefaultProfile()
    {
        CreateMap<UserDto, User>();
        CreateMap<User, UserDto>();

        CreateMap<BuyingListDto, BuyingList>();
        CreateMap<BuyingList, BuyingListDto>();

        // Example with more details
        //CreateMap<Source, Destination>()
        //    .ForMember(dest => dest.Name, o => o.MapFrom(src => src.FirstName))
        //    .ForMember(dest => dest.Identity, o => o.MapFrom(src => src.Id))
        //    .ForMember(dest => dest.Location, o => o.MapFrom(src => src.Address));
    }
}

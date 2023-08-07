using AutoMapper;
using FluentValidation;
using MyBuyingList.Application.Common.Extensions;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.DTOs;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Services;

public class BuyingListService : IBuyingListService
{
    private readonly IBuyingListRepository _buyingListRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<BuyingListDto> _validator;

    public BuyingListService(IBuyingListRepository buyingListRepository, IMapper mapper, IValidator<BuyingListDto> validator)
    {
        _buyingListRepository = buyingListRepository;
        _mapper = mapper;
        _validator = validator;
    }
    public BuyingListDto? GetById(int id)
    {
        return _mapper.Map<BuyingListDto>(_buyingListRepository.Get(id));
    }

    public void Create(BuyingListDto buyingListDto)
    {
        _validator.ValidateAndThrowCustomException(buyingListDto);
        _buyingListRepository.Add(_mapper.Map<BuyingList>(buyingListDto));
    }
    
    public void Update(BuyingListDto buyingListDto)
    {
        _validator.ValidateAndThrowCustomException(buyingListDto);
        _buyingListRepository.Edit(_mapper.Map<BuyingList>(buyingListDto));
    }
    
    public void Delete(BuyingListDto buyingListDto)
    {
        _validator.ValidateAndThrowCustomException(buyingListDto);
        _buyingListRepository.Delete(_mapper.Map<BuyingList>(buyingListDto));
    }
}

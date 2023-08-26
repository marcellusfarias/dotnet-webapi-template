using AutoMapper;
using FluentValidation;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.Contracts.BuyingListDtos;
using MyBuyingList.Application.DTOs.UserDtos;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Services;

public class BuyingListService : IBuyingListService
{
    private readonly IBuyingListRepository _buyingListRepository;
    private readonly IMapper _mapper;

    public BuyingListService(IBuyingListRepository buyingListRepository, IMapper mapper)
    {
        _buyingListRepository = buyingListRepository;
        _mapper = mapper;
    }

    public GetBuyingListDto? GetById(int id)
    {
        var buyingList = _buyingListRepository.Get(id);
        return buyingList == null
            ? throw new ResourceNotFoundException()
            : _mapper.Map<GetBuyingListDto>(buyingList);
    }

    public int Create(CreateBuyingListDto buyingListDto, int currentUserId)
    {
        var buyingList = _mapper.Map<BuyingList>(buyingListDto);
        buyingList.CreatedBy = currentUserId;

        return _buyingListRepository.Add(buyingList);
    }

    public void ChangeName(UpdateBuyingListNameDto dto)
    {
        var buyingList = _buyingListRepository.Get(dto.Id);
        if (buyingList is null)
            throw new ResourceNotFoundException();

        buyingList.Name = dto.Name;

        _buyingListRepository.Edit(buyingList);
    }

    public void Delete(int buyingListId)
    {
        var buyingList = _buyingListRepository.Get(buyingListId);

        if (buyingList == null)
            throw new ResourceNotFoundException();

        _buyingListRepository.DeleteBuyingListAndItems(buyingList);
    }    
}

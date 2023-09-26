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

    public async Task<GetBuyingListDto?> GetByIdAsync(int id)
    {
        BuyingList? buyingList = await _buyingListRepository.GetAsync(id);
        return buyingList == null
            ? throw new ResourceNotFoundException()
            : _mapper.Map<BuyingList, GetBuyingListDto>(buyingList);
    }

    public async Task<int> CreateAsync(CreateBuyingListDto buyingListDto, int currentUserId)
    {
        var buyingList = _mapper.Map<BuyingList>(buyingListDto);
        buyingList.CreatedBy = currentUserId;

        return await _buyingListRepository.AddAsync(buyingList);
    }

    public async Task ChangeNameAsync(UpdateBuyingListNameDto dto)
    {
        var buyingList = await _buyingListRepository.GetAsync(dto.Id);
        if (buyingList is null)
            throw new ResourceNotFoundException();

        buyingList.Name = dto.Name;

        await _buyingListRepository.EditAsync(buyingList);
    }

    public async Task DeleteAsync(int buyingListId)
    {
        var buyingList = await _buyingListRepository.GetAsync(buyingListId);

        if (buyingList == null)
            throw new ResourceNotFoundException();

        await _buyingListRepository.DeleteBuyingListAndItemsAsync(buyingList);
    }
}

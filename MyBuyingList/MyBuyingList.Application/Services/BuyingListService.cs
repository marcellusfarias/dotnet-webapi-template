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

    public async Task<GetBuyingListDto?> GetByIdAsync(int id, CancellationToken token)
    {
        BuyingList? buyingList = await _buyingListRepository.GetAsync(id, token);
        return buyingList == null
            ? throw new ResourceNotFoundException()
            : _mapper.Map<BuyingList, GetBuyingListDto>(buyingList);
    }

    public async Task<int> CreateAsync(CreateBuyingListDto buyingListDto, int currentUserId, CancellationToken token)
    {
        var buyingList = _mapper.Map<BuyingList>(buyingListDto);
        buyingList.CreatedBy = currentUserId;

        return await _buyingListRepository.AddAsync(buyingList, token);
    }

    public async Task ChangeNameAsync(UpdateBuyingListNameDto dto, CancellationToken token)
    {
        var buyingList = await _buyingListRepository.GetAsync(dto.Id, token);
        if (buyingList is null)
            throw new ResourceNotFoundException();

        buyingList.Name = dto.Name;

        await _buyingListRepository.EditAsync(buyingList, token);
    }

    public async Task DeleteAsync(int buyingListId, CancellationToken token)
    {
        var buyingList = await _buyingListRepository.GetAsync(buyingListId, token);

        if (buyingList == null)
            throw new ResourceNotFoundException();

        await _buyingListRepository.DeleteBuyingListAndItemsAsync(buyingList, token);
    }
}

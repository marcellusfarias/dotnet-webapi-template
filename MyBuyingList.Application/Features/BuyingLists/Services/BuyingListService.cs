using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Features.BuyingLists.DTOs;
using MyBuyingList.Application.Features.BuyingLists.Mappers;
using MyBuyingList.Application.Features.Groups;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Features.BuyingLists.Services;

public class BuyingListService : IBuyingListService
{
    private readonly IBuyingListRepository _buyingListRepository;
    private readonly IGroupRepository _groupRepository;

    public BuyingListService(IBuyingListRepository buyingListRepository, IGroupRepository groupRepository)
    {
        _buyingListRepository = buyingListRepository;
        _groupRepository = groupRepository;
    }

    public async Task<GetBuyingListDto?> GetByIdAsync(int id, CancellationToken token)
    {
        BuyingList? buyingList = await _buyingListRepository.GetAsync(id, token);

        if (buyingList is null)
            throw new ResourceNotFoundException();

        return buyingList.ToGetBuyingListDto();
    }

    public async Task<int> CreateAsync(CreateBuyingListDto createBuyingListDto, int currentUserId, CancellationToken token)
    {
        var buyingList = createBuyingListDto.ToBuyingList(currentUserId, DateTime.Now);

        var group = await _groupRepository.GetAsync(buyingList.GroupId, token);

        if (group is null)
            throw new BusinessLogicException("Cannot create buying list. Invalid group.");

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

        if (buyingList is null)
            throw new ResourceNotFoundException();

        await _buyingListRepository.DeleteBuyingListAndItemsAsync(buyingList, token);
    }
}

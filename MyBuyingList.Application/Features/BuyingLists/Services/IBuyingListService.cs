using MyBuyingList.Application.Features.BuyingLists.DTOs;

namespace MyBuyingList.Application.Features.BuyingLists.Services;

public interface IBuyingListService
{
    Task<GetBuyingListDto?> GetByIdAsync(int id, CancellationToken token);
    /// <summary>
    /// Creates new buying list.
    /// </summary>
    /// <param name="buyingListDto"></param>
    /// <param name="currentUserId"></param>
    /// <returns>The new list Id.</returns>
    Task<int> CreateAsync(CreateBuyingListDto buyingListDto, int currentUserId, CancellationToken token);
    Task ChangeNameAsync(UpdateBuyingListNameDto dto, CancellationToken token);
    /// <summary>
    /// Delete buying list and items associated.
    /// </summary>
    /// <param name="buyingListId"></param>
    /// <exception cref="">Throws ResourceNotFound</exception>
    Task DeleteAsync(int buyingListId, CancellationToken token);
}

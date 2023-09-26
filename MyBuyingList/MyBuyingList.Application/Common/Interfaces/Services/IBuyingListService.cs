using MyBuyingList.Application.Contracts.BuyingListDtos;
using MyBuyingList.Application.DTOs;

namespace MyBuyingList.Application.Common.Interfaces.Services;

public interface IBuyingListService
{
    Task<GetBuyingListDto?> GetByIdAsync(int id);
    /// <summary>
    /// Creates new buying list.
    /// </summary>
    /// <param name="buyingListDto"></param>
    /// <param name="currentUserId"></param>
    /// <returns>The new list Id.</returns>
    Task<int> CreateAsync(CreateBuyingListDto buyingListDto, int currentUserId);
    Task ChangeNameAsync(UpdateBuyingListNameDto dto);
    /// <summary>
    /// Delete buying list and items associated.
    /// </summary>
    /// <param name="buyingListId"></param>
    /// <exception cref="">Throws ResourceNotFound</exception>
    Task DeleteAsync(int buyingListId);
}

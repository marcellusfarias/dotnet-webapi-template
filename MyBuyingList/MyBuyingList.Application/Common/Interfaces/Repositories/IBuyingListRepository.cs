using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Common.Interfaces.Repositories;

public interface IBuyingListRepository : IRepository<BuyingList>
{
    void DeleteBuyingListAndItems(BuyingList buyingList);
    Task DeleteBuyingListAndItemsAsync(BuyingList buyingList);
}

using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Common.Interfaces.Repositories;

public interface IBuyingListRepository : IRepository<BuyingList>
{
    Task DeleteBuyingListAndItemsAsync(BuyingList buyingList, CancellationToken token);
}

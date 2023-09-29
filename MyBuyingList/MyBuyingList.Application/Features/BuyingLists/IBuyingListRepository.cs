using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Features.BuyingLists;

public interface IBuyingListRepository : IRepository<BuyingList>
{
    Task DeleteBuyingListAndItemsAsync(BuyingList buyingList, CancellationToken token);
}

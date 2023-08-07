using MyBuyingList.Application.DTOs;

namespace MyBuyingList.Application.Common.Interfaces.Services;

public interface IBuyingListService
{
    BuyingListDto? GetById(int id);
    void Create(BuyingListDto buyingListDto);
    void Update(BuyingListDto buyingListDto);
    void Delete(BuyingListDto buyingListDto);
}

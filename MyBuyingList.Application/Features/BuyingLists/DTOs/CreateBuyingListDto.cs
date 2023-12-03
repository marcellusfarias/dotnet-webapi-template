namespace MyBuyingList.Application.Features.BuyingLists.DTOs;

public class CreateBuyingListDto
{
    public required string Name { get; set; }
    public required int GroupId { get; set; }
}
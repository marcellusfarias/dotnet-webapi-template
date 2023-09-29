namespace MyBuyingList.Application.Features.BuyingLists.DTOs;

public class GetBuyingListDto
{
    public required string Name { get; set; }
    public required int GroupId { get; set; }
    public required int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
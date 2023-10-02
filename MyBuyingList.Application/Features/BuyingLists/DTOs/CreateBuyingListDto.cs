namespace MyBuyingList.Application.Features.BuyingLists.DTOs;

//Since I'm using DTO for incoming and outcoming data, I made some properties nullable
//so the user does not specify them on Creating a BuyingList
public class CreateBuyingListDto
{
    public required string Name { get; set; }
    public required int GroupId { get; set; }
}
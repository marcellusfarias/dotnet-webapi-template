namespace MyBuyingList.Application.Features.Users.DTOs;

public class GetUserDto
{
    public required int Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required bool Active { get; set; }
}
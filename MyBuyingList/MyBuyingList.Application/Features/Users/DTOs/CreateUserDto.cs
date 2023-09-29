namespace MyBuyingList.Application.Features.Users.DTOs;

public class CreateUserDto
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}

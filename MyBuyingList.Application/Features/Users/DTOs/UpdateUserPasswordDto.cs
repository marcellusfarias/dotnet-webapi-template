namespace MyBuyingList.Application.Features.Users.DTOs;

public class UpdateUserPasswordDto
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}
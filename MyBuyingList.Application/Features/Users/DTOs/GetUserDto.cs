namespace MyBuyingList.Application.Features.Users.DTOs;

public record GetUserDto(int Id, string UserName, string Email, bool Active);
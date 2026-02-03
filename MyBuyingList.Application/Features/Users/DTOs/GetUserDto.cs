using MyBuyingList.Domain.ValueObjects;

namespace MyBuyingList.Application.Features.Users.DTOs;

public record GetUserDto(int Id, string UserName, EmailAddress Email, bool Active);
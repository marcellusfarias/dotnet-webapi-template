using MyBuyingList.Domain.ValueObjects;

namespace MyBuyingList.Application.Features.Users.DTOs;

public record UserDto(int Id, string UserName, EmailAddress Email, bool Active);
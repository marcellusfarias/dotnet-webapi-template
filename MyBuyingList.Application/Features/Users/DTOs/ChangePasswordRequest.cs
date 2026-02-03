namespace MyBuyingList.Application.Features.Users.DTOs;

public record ChangePasswordRequest(string OldPassword, string NewPassword);
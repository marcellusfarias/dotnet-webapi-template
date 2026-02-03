namespace MyBuyingList.Application.Features.Users.DTOs;

public record CreateUserRequest
{
    public string UserName { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }

    public CreateUserRequest(string username, string email, string password)
    {
        UserName = username.ToLower();
        Email = email.ToLower();
        Password = password;
    }
}
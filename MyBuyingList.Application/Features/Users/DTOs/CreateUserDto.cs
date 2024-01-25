namespace MyBuyingList.Application.Features.Users.DTOs;

public record CreateUserDto
{
    public string UserName { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }

    public CreateUserDto(string username, string email, string password)
    {
        UserName = username.ToLower();
        Email = email.ToLower();
        Password = password;
    }
}
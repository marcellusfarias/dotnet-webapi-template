namespace MyBuyingList.Application.Common.Constants;

// Custom validation messages for FluentValidation
public static class ValidationMessages
{
    public const string InvalidUsername = "Username must only contain letters, numbers or underscores";
    public const string InvalidEmail = "Invalid email, please review.";
    public const string InvalidPassword = "Invalid password. Passwords should contain at least one upper case letter, one lower case letter, one number, one special character and its length be between 8 and 32 characters long.";
}

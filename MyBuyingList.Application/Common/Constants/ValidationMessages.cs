namespace MyBuyingList.Application.Common.Constants;

// Custom validation messages for FluentValidation
public static class ValidationMessages
{
    public static string INVALID_USERNAME = "Username must only contain letters, numbers or underscores";
    public static string INVALID_EMAIL = "Invalid email, please review.";
    public static string INVALID_PASSWORD = "Invalid password. Passwords should contain at least one upper case letter, one lower case letter, one number, one special character and its length be between 8 and 32 characters long.";
}

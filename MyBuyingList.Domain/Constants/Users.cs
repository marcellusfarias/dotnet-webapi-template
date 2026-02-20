using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Domain.Constants;

public static class Users
{
    public static readonly User AdminUser = new()
    {
        Id = 1,
        UserName = "admin",
        // Email and Password are sourced from secrets at runtime via AdminUserSeeder.
        // These placeholder values are never persisted.
        Email = string.Empty,
        Password = string.Empty,
        Active = true
    };
}

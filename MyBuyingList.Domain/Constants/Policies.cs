using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Domain.Constants;

public static class Policies
{
    public const string UserCreate = "UserCreate";
    public const string UserUpdate = "UserUpdate";
    public const string UserDelete = "UserDelete";
    public const string UserGetAll = "UserGetAll";
    public const string UserGet = "UserGet";

    public static List<Policy> GetValues() =>
    [
        new() { Id = 1, Name = UserCreate },
        new() { Id = 2, Name = UserUpdate },
        new() { Id = 3, Name = UserDelete },
        new() { Id = 4, Name = UserGetAll },
        new() { Id = 5, Name = UserGet }
    ];
}

using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Domain.Constants;

public static class Policies
{
    public const string UserCreate = "UserCreate";
    public const string UserUpdate = "UserUpdate";
    public const string UserDelete = "UserDelete";
    public const string UserGetAll = "UserGetAll";
    public const string UserGet = "UserGet";

    public static IEnumerable<Policy> GetValues()
    {
        var orderedDictionary = new Dictionary<int, string>()
        {
            {1, UserCreate},
            {2, UserUpdate},
            {3, UserDelete},
            {4, UserGetAll},
            {5, UserGet},
        };

        List<Policy> policies = [];

        // doing this, because one can not guarantee order using reflection.
        // Knowing the ID may be important on some use cases.
        foreach (var item in orderedDictionary)
        {
            policies.Add(new Policy { Id = item.Key, Name = item.Value });
        }

        return policies;
    }
}

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
        var _ordernedDictionary = new Dictionary<int, string>()
        {
            #region Items
            {1, UserCreate},
            {2, UserUpdate},
            {3, UserDelete},
            {4, UserGetAll},
            {5, UserGet},
            #endregion
        };

        List<Policy> policies = new List<Policy>();

        // doing this, because one can not guarantee order using reflection.
        // Knowing the ID may be important on some use cases.
        foreach (var item in _ordernedDictionary)
        {
            policies.Add(new Policy { Id = item.Key, Name = item.Value });
        }

        return policies;
    }
}

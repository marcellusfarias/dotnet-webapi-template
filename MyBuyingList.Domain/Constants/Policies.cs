using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Domain.Constants;

public static class Policies
{
    public const string CreateUser = "CreateUser";
    public const string UpdateUser = "UpdateUser";
    public const string DeleteUser = "DeleteUser";
    public const string GetAllUsers = "GetAllUsers";
    public const string GetUser = "GetUser";

    public const string BuyingListGet = "BuyingListGet";
    public const string BuyingListCreate = "BuyingListCreate";
    public const string BuyingListUpdate = "BuyingListUpdate";
    public const string BuyingListDelete = "BuyingListDelete";

    public const string GroupGet = "GroupGet";
    public const string GroupCreate = "GroupCreate";
    public const string GroupUpdate = "GroupUpdate";
    public const string GroupDelete = "GroupDelete";

    public static IEnumerable<Policy> GetValues()
    {
        var _ordernedDictionary = new Dictionary<int, string>()
        {
            #region Items
            {1, CreateUser},
            {2, UpdateUser},
            {3, DeleteUser},
            {4, GetAllUsers},
            {5, GetUser},
            {6, BuyingListGet},
            {7, BuyingListCreate},
            {8, BuyingListUpdate},
            {9, BuyingListDelete},
            {10, GroupGet},
            {11, GroupCreate},
            {12, GroupUpdate},
            {13, GroupDelete},
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

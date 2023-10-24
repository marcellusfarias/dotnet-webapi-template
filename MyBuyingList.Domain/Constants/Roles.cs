using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Domain.Constants;

public static class Roles
{
    public const string Administrator = "Administrator";
    public const string RegularUser = "RegularUser";

    public static IEnumerable<Role> GetValues()
    {
        var _ordernedDictionary = new Dictionary<int, string>()
        {
            #region Items
            {1, Administrator},
            {2, RegularUser}
            #endregion
        };

        List<Role> roles = new List<Role>();

        // doing this, because one can not guarantee order using reflection.
        // Knowing the ID may be important on some use cases.
        foreach (var item in _ordernedDictionary)
        {
            roles.Add(new Role { Id = item.Key, Name = item.Value });
        }

        return roles;
    }
}

using MyBuyingList.Domain.Entities;
using System.Reflection;

namespace MyBuyingList.Infrastructure.Auth.Constants;

//maybe this should be on the Domain project
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

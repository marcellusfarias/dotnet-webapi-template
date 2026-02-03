using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Domain.Constants;

public static class Roles
{
    public const string Administrator = "Administrator";
    public const string RegularUser = "RegularUser";

    public static IEnumerable<Role> GetValues()
    {
        var orderedDictionary = new Dictionary<int, string>()
        {
            {1, Administrator},
            {2, RegularUser}
        };

        List<Role> roles = [];

        // doing this, because one can not guarantee order using reflection.
        // Knowing the ID may be important on some use cases.
        foreach (var item in orderedDictionary)
        {
            roles.Add(new Role { Id = item.Key, Name = item.Value });
        }

        return roles;
    }
}

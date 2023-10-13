using MyBuyingList.Domain.Entities;
using System.Reflection;

namespace MyBuyingList.Infrastructure.Auth.Constants;

//maybe this should be on the Domain project
public static class Roles
{
    public const string Administrator = "Administrator";
    public const string RegularUser = "RegularUser";

    private static Dictionary<int, string> _ordernedDictionary = new Dictionary<int, string>()
    {
        {1, Administrator},
        {2, RegularUser}
    };

    public static IEnumerable<Role> GetValues()
    {
        List<Role> roles = new List<Role>();

        // doing this, because one can not guarantee order using reflection.
        // Knowing the ID may be important on some use cases.
        foreach (var item in _ordernedDictionary)
        {
            roles.Add(new Role { Id = item.Key, Name = item.Value });
        }

        //FieldInfo[] fieldInfos = typeof(Roles).GetFields(BindingFlags.Public |
        //    BindingFlags.Static | BindingFlags.FlattenHierarchy);
        //fieldInfos
        //    .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
        //    .ToList()
        //    //.OrderBy(x => x.Name) order not guarenteed
        //    .ForEach(x => roles.Add(new Role { Id = currentId++, Name = (string)x.GetRawConstantValue()! }));

        return roles;
    }
}

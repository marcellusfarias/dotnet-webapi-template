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
        List<Role> roles = new List<Role>();
        int currentId = 1; // auto generated ID not working on Postgres....

        FieldInfo[] fieldInfos = typeof(Roles).GetFields(BindingFlags.Public |
            BindingFlags.Static | BindingFlags.FlattenHierarchy);
        fieldInfos
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .ToList()
            //.OrderBy(x => x.Name) order not guarenteed
            .ForEach(x => roles.Add(new Role { Id = currentId++, Name = (string)x.GetRawConstantValue()! }));

        return roles;
    }
}

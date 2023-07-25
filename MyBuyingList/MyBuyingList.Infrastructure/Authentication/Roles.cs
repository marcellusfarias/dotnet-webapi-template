using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Infrastructure.Authentication;

public static class Roles
{
    public const string Administrator = "Administrator";

    public static IEnumerable<Role> GetValues()
    {
        List<Role> roles = new List<Role>();
        int currentId = 1; // auto generated ID not working on Postgres....

        FieldInfo[] fieldInfos = typeof(Roles).GetFields(BindingFlags.Public |
            BindingFlags.Static | BindingFlags.FlattenHierarchy);
        fieldInfos
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .ToList()
            .ForEach(x => roles.Add(new Role { Id = currentId++, Name = (string) x.GetRawConstantValue()! } ));

        return roles;
    }
}

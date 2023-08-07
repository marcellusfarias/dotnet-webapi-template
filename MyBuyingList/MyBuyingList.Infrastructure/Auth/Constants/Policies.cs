using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Infrastructure.Auth.Constants;

//maybe this should be on the Domain project
public static class Policies
{
    public const string CreateUser = "CreateUser";
    public const string UpdateUser = "UpdateUser";
    public const string DeleteUser = "DeleteUser";
    public const string GetAllUsers = "GetAllUsers";

    public const string BuyingListGet = "BuyingListGet";
    public const string BuyingListCreate = "BuyingListCreate";
    public const string BuyingListUpdate = "BuyingListUpdate";
    public const string BuyingListDelete = "BuyingListDelete";

    public static IEnumerable<Policy> GetValues()
    {
        List<Policy> policies = new List<Policy>();
        int currentId = 1; // auto generated ID not working on Postgres....

        FieldInfo[] fieldInfos = typeof(Policies).GetFields(BindingFlags.Public |
            BindingFlags.Static | BindingFlags.FlattenHierarchy);
        fieldInfos
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .ToList()
            //.OrderBy(x => x.Name) order not guarenteed
            .ForEach(x => policies.Add(new Policy { Id = currentId++, Name = (string)x.GetRawConstantValue()! }));

        return policies;
    }
}

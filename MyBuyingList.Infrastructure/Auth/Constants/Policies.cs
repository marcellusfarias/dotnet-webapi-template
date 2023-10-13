using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
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

    public const string GroupGet = "GroupGet";
    public const string GroupCreate = "GroupCreate";
    public const string GroupUpdate = "GroupUpdate";
    public const string GroupDelete = "GroupDelete";

    private static Dictionary<int, string> _ordernedDictionary = new Dictionary<int, string>()
    {
        {1, CreateUser},
        {2, UpdateUser},
        {3, DeleteUser},
        {4, GetAllUsers},
        {5, BuyingListGet},
        {6, BuyingListCreate},
        {7, BuyingListUpdate},
        {8, BuyingListDelete},
        {9, GroupGet},
        {10, GroupCreate},
        {11, GroupUpdate},
        {12, GroupDelete},
    };

    public static IEnumerable<Policy> GetValues()
    {
        List<Policy> policies = new List<Policy>();

        // doing this, because one can not guarantee order using reflection.
        // Knowing the ID may be important on some use cases.
        foreach (var item in _ordernedDictionary)
        {
            policies.Add(new Policy { Id = item.Key, Name = item.Value });
        }

        //FieldInfo[] fieldInfos = typeof(Policies).GetFields(BindingFlags.Public |
        //    BindingFlags.Static | BindingFlags.FlattenHierarchy);
        //fieldInfos
        //    .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
        //    .ToList()
        //    //.OrderBy(x => x.Name) order not guarenteed
        //    .ForEach(x => policies.Add(new Policy { Id = currentId++, Name = (string)x.GetRawConstantValue()! }));

        return policies;
    }
}

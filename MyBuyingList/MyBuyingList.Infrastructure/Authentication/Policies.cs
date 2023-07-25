﻿using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Infrastructure.Authentication;

public static class Policies
{
    public const string CreateUser = "CreateUser";
    public const string UpdateUser = "UpdateUser";
    public const string DeleteUser = "DeleteUser";

    public static IEnumerable<Policy> GetValues()
    {
        List<Policy> policies = new List<Policy>();
        int currentId = 1; // auto generated ID not working on Postgres....

        FieldInfo[] fieldInfos = typeof(Policies).GetFields(BindingFlags.Public |
            BindingFlags.Static | BindingFlags.FlattenHierarchy);
        fieldInfos
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .ToList()
            .ForEach(x => policies.Add(new Policy { Id = currentId++,  Name = (string)x.GetRawConstantValue()! }));

        return policies;
    }
}

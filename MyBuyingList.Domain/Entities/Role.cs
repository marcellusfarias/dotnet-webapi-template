﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Domain.Entities;

public class Role : BaseEntity
{
    public required string Name { get; set; }

    //Entity Framework navigation property.
    //Not sure if it should stay in domain. See: https://softwareengineering.stackexchange.com/questions/396043/domain-driven-design-navigation-properties-and-aggregate
#pragma warning disable CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<RolePolicy> RolePolicies { get; set; }
#pragma warning restore CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.

}

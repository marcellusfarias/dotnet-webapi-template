using MyBuyingList.Domain.Common;

namespace MyBuyingList.Domain.Entities;

public sealed class Role : BaseEntity
{
    public required string Name { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RolePolicy> RolePolicies { get; set; } = [];
}

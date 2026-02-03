using MyBuyingList.Domain.Common;

namespace MyBuyingList.Domain.Entities;

public sealed class Policy : BaseEntity
{
    public required string Name { get; set; }

    public ICollection<RolePolicy> RolePolicies { get; set; } = [];
}

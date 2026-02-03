using MyBuyingList.Domain.Common;

namespace MyBuyingList.Domain.Entities;

public sealed class RolePolicy : BaseEntity
{
    public required int RoleId { get; set; }
    public required int PolicyId { get; set; }
    
    public Role Role { get; set; } = null!;
    public Policy Policy { get; set; } = null!;
}

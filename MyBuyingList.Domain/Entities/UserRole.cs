using MyBuyingList.Domain.Common;

namespace MyBuyingList.Domain.Entities;

public sealed class UserRole : BaseEntity
{
    public required int UserId { get; set; }
    public required int RoleId { get; set; }

    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}

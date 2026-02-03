using MyBuyingList.Domain.Common;

namespace MyBuyingList.Domain.Entities;

public sealed class User : BaseEntity
{
    public required string Email { get; set; }
    public required string UserName { get; set; }   
    public required string Password { get; set; }
    public DateTime CreatedAt { get; set; } //not required because it will be set on postgres with default value.... think about this
    public required bool Active { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
}

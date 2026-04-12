using MyBuyingList.Domain.Common;

namespace MyBuyingList.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public required int UserId { get; set; }
    public required string TokenHash { get; set; }
    public required DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsRevoked { get; set; }

    public User User { get; set; } = null!;
}

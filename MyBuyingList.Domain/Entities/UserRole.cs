namespace MyBuyingList.Domain.Entities;

public class UserRole : BaseEntity
{
    public required int UserId { get; set; }
    public required int RoleId { get; set; }

    //Entity Framework navigation property.
    //Not sure if it should stay in domain. See: https://softwareengineering.stackexchange.com/questions/396043/domain-driven-design-navigation-properties-and-aggregate
#pragma warning disable CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
    public virtual User User { get; set; }
    public virtual Role Role { get; set; }
#pragma warning restore CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.

}

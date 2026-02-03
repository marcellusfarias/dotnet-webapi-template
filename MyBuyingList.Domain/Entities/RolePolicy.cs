namespace MyBuyingList.Domain.Entities;

public class RolePolicy : BaseEntity
{
    public required int RoleId { get; set; }
    public required int PolicyId { get; set; }

#pragma warning disable CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.

    public virtual Role Role { get; set; }
    public virtual Policy Policy { get; set; }
#pragma warning restore CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.

}

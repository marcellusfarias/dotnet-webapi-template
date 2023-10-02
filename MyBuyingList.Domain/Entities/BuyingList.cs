using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Domain.Entities;

public class BuyingList : BaseEntity
{
    public required string Name { get; set; }
    public required int GroupId { get; set; }
    public required int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } //not required because it will be set on postgres with default value.... think about this

    //Entity Framework navigation property.
    //Not sure if it should stay in domain. See: https://softwareengineering.stackexchange.com/questions/396043/domain-driven-design-navigation-properties-and-aggregate
#pragma warning disable CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
    public virtual Group Group { get; set; }
    public virtual User UserCreated { get; set; }
    public virtual ICollection<BuyingListItem> Items { get; set; }
#pragma warning restore CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.

}

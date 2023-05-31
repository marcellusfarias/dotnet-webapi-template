using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Domain.Entities;

public class BuyingList : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public required int GroupId { get; set; }
    public required int CreatedBy { get; set; }
    public required DateTime CreatedAt { get; set; }
    public Group Group { get; set; }    
    public User UserCreated { get; set; }
    public ICollection<BuyingListItem> Items { get; set; }
}

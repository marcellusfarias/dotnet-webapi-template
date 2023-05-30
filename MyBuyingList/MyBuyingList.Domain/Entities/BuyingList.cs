using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Domain.Entities;

public class BuyingList : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int GroupId { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Group Group { get; set; }    
    public User UserCreated { get; set; }
    public ICollection<BuyingListItem> Items { get; set; }
}

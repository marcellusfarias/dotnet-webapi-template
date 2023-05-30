using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Domain.Entities;

public class BuyingListItem : BaseEntity
{
    public int BuyingListId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public BuyingList BuyingList { get; set; }
}

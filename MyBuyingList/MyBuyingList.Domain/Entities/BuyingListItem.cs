using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Domain.Entities;

public class BuyingListItem : BaseEntity
{
    public int BuyingListId { get; set; }
    public required string Description { get; set; }
    public bool Completed { get; set; }
    public BuyingList BuyingList { get; set; }
}

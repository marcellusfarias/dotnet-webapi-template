using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Domain.Entities;

public class Group : BaseAuditableEntity
{
    public string GroupName { get; set; } = string.Empty;
    public User User { get; set; }
    public ICollection<BuyingList> BuyingLists { get; set; }
}

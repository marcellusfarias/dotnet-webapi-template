using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Domain.Entities;

public class User : BaseEntity
{
    public required string Email { get; set; }
    public required string UserName { get; set; }   
    public required string Password { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required bool Active { get; set; }
    public ICollection<Group> GroupsCreatedBy { get; set; }
    public ICollection<BuyingList> BuyingListCreatedBy { get; set; }
}

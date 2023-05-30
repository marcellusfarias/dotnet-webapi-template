﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;     
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool Active { get; set; }
    public ICollection<Group> GroupsCreatedBy { get; set; }
    public ICollection<BuyingList> BuyingListCreatedBy { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBuyingList.Domain.Entities;


namespace MyBuyingList.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Group> Groups { get; }
    DbSet<BuyingList> BuyingLists { get; }
    DbSet<BuyingListItem> BuyingListItems { get; }

}

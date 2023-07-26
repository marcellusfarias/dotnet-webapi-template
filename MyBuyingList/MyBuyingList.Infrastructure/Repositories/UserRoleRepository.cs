using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Infrastructure.Repositories;

internal class UserRoleRepository : RepositoryBase<UserRole>
{
    public UserRoleRepository(ApplicationDbContext context) : base(context)
    {
    }
}

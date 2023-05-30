using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    IEnumerable<User> GetActiveUsers();
}

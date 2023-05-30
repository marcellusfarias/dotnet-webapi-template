using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MyBuyingList.Infrastructure.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    //Make this async
    public IEnumerable<User> GetActiveUsers()
    {
        //if you want to use Dapper for performance issues, see below
        //_context.QueryAsync(ct, "SELECT * FROM users WHERE Active = 'true';");
        var result = _context.Set<User>().Where(x => x.Active).ToList();
        return result;
    }
}

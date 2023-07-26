using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MyBuyingList.Application.Common.Exceptions;

namespace MyBuyingList.Infrastructure.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    //Make this async
    public IEnumerable<User> GetActiveUsers()
    {
        try
        {
            //if you want to use Dapper for performance issues, see below
            //_context.QueryAsync(ct, "SELECT * FROM users WHERE Active = 'true';");
            var result = _context.Set<User>().Where(x => x.Active).ToList();
            return result;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }

    public User? GetAuthenticationDataByEmail(string email)
    {
        try
        {
            return _context.Set<User>().Where(x => x.Email == email && x.Active).Single();
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }       
    }

}

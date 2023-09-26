using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MyBuyingList.Application.Common.Exceptions;
using static Dapper.SqlMapper;

namespace MyBuyingList.Infrastructure.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        try
        {
            //if you want to use Dapper for performance issues, see below
            //_context.QueryAsync(ct, "SELECT * FROM users WHERE Active = 'true';");
            var result = await _context.Set<User>().Where(x => x.Active).ToListAsync();
            return result;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }

    //test this
    public async Task LogicalExclusionAsync(User user)
    {
        try
        {
            user.Active = false;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }
}

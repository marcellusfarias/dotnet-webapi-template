using MyBuyingList.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Features.Users;

namespace MyBuyingList.Infrastructure.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetActiveUserByUsername(string username, CancellationToken token)
    {
        try
        {
            //if you want to use Dapper for performance issues, see below
            //_context.QueryAsync(ct, "SELECT * FROM users WHERE Active = 'true';");
            var result = await _context.Set<User>()
                .Where(x => x.Active && x.UserName.Equals(username))
                .AsNoTracking()
                .FirstOrDefaultAsync(token);

            return result;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken token)
    {
        try
        {
            //if you want to use Dapper for performance issues, see below
            //_context.QueryAsync(ct, "SELECT * FROM users WHERE Active = 'true';");
            var result = await _context.Set<User>()
                .Where(x => x.Active)
                .AsNoTracking()
                .ToListAsync(token);

            return result;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }

    public async Task<List<Policy>?> GetUserPolicies(int id, CancellationToken token)
    {
        #region Not Great Query example
        //SELECT t.id, t.active, t.created_at, t.email, t.password, t.user_name, t0.id, t0.role_id, t0.user_id, t0.id0, t0.name, t0.id1, t0.policy_id, t0.role_id0, t0.id00, t0.name0
        //FROM(
        //    SELECT u.id, u.active, u.created_at, u.email, u.password, u.user_name
        //    FROM users AS u
        //    WHERE u.id = @__id_0
        //    LIMIT 1
        //        ) AS t
        //LEFT JOIN(
        //    SELECT u0.id, u0.role_id, u0.user_id, r.id AS id0, r.name, t1.id AS id1, t1.policy_id, t1.role_id AS role_id0, t1.id0 AS id00, t1.name AS name0
        //    FROM user_roles AS u0
        //    INNER JOIN roles AS r ON u0.role_id = r.id
        //    LEFT JOIN (
        //        SELECT r0.id, r0.policy_id, r0.role_id, p.id AS id0, p.name
        //        FROM role_policies AS r0
        //        INNER JOIN policies AS p ON r0.policy_id = p.id
        //    ) AS t1 ON r.id = t1.role_id
        //) AS t0 ON t.id = t0.user_id
        //ORDER BY t.id, t0.id, t0.id0, t0.id1
        //var query = _context
        //                .Set<User>()
        //                .Where(x => x.Id == id)
        //                .Include(user => user.UserRoles)
        //                .ThenInclude(userRole => userRole.Role)
        //                .ThenInclude(role => role.RolePolicies)
        //                .ThenInclude(rolePolicy => rolePolicy.Policy)

        //return await query.FirstOrDefaultAsync(token);

        // better than upper approach
        // SELECT p.id, p.name
        // FROM users AS u
        // INNER JOIN user_roles AS u0 ON u.id = u0.user_id
        // INNER JOIN roles AS r ON u0.role_id = r.id
        // INNER JOIN role_policies AS r0 ON r.id = r0.role_id
        // INNER JOIN policies AS p ON r0.policy_id = p.id
        // WHERE u.id = @__id_0
        #endregion
        try
        {
            var policies = await _context.Set<User>()
                            .Where(user => user.Id == id)
                            .SelectMany(user => user.UserRoles)
                            .Select(userRole => userRole.Role)
                            .SelectMany(role => role.RolePolicies)
                            .Select(rolePolicy => rolePolicy.Policy)
                            .AsNoTracking()
                            .ToListAsync(token);

            return policies;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }

    //test this
    public async Task LogicalExclusionAsync(User user, CancellationToken token)
    {
        try
        {
            user.Active = false;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync(token);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }
}

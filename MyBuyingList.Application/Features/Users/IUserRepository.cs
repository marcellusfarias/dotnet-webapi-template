using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Features.Users;

public interface IUserRepository : IRepository<User>
{
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken token);
    Task<User?> GetActiveUserByUsername(string username, CancellationToken token);
    Task<List<Policy>?> GetUserPolicies(int id, CancellationToken token);
    Task LogicalExclusionAsync(User user, CancellationToken token);
}

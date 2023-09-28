using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken token);
    Task<List<Policy>?> GetUserPolicies(int id, CancellationToken token);
    Task LogicalExclusionAsync(User user, CancellationToken token);
}

using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Features.Users;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetActiveUserByUsernameAsync(string username, CancellationToken token);
    Task<List<Policy>?> GetUserPoliciesAsync(int id, CancellationToken token);
    Task LogicalExclusionAsync(User user, CancellationToken token);
}

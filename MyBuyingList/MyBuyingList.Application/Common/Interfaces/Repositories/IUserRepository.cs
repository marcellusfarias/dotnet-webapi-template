using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    IEnumerable<User> GetActiveUsers();
    Task<IEnumerable<User>> GetActiveUsersAsync();
    void LogicalExclusion(User user);
    Task LogicalExclusionAsync(User user);
}

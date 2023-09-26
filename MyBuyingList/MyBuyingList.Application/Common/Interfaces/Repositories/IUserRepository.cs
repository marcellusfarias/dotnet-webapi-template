using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task LogicalExclusionAsync(User user);
}

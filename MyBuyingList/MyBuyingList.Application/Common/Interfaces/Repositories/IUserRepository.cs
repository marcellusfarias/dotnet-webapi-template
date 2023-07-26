using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    IEnumerable<User> GetActiveUsers();
    //will provide claims also
    User? GetAuthenticationDataByEmail(string email);
}

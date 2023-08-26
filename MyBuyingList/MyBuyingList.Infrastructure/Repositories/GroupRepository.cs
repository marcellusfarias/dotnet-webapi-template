using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Infrastructure.Repositories;

public class GroupRepository : RepositoryBase<Group>, IGroupRepository
{
    public GroupRepository(ApplicationDbContext context) : base(context) { }
}

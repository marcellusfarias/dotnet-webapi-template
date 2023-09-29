using MyBuyingList.Application.Features.Groups;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Infrastructure.Repositories;

public class GroupRepository : RepositoryBase<Group>, IGroupRepository
{
    public GroupRepository(ApplicationDbContext context) : base(context) { }
}

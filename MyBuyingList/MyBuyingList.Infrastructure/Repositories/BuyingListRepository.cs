using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Domain.Entities;
using static Dapper.SqlMapper;

namespace MyBuyingList.Infrastructure.Repositories;

public class BuyingListRepository : RepositoryBase<BuyingList>, IBuyingListRepository
{
    public BuyingListRepository(ApplicationDbContext context) : base(context) { }

}

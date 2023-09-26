using MyBuyingList.Domain.Common;

namespace MyBuyingList.Application.Common.Interfaces.Repositories;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Adds a new entity in the database asyncronously.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>Returns the Id of the added entity.</returns>
    Task<int> AddAsync(TEntity entity);
    Task AddRangeAsync(ICollection<TEntity> entities);
    Task DeleteAsync(TEntity entity);
    Task DeleteRangeAsync(ICollection<TEntity> entities);
    Task EditAsync(TEntity entity);
}

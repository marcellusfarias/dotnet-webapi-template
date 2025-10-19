using MyBuyingList.Domain.Common;

namespace MyBuyingList.Application.Common.Interfaces;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetAsync(int id, CancellationToken cancellationToken);
    Task<List<TEntity>> GetAllAsync(int page, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new entity in the database asyncronously.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the Id of the added entity.</returns>
    Task<int> AddAsync(TEntity entity, CancellationToken cancellationToken);
    Task AddRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    Task DeleteRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken);
    Task EditAsync(TEntity entity, CancellationToken cancellationToken);
}

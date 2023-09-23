using MyBuyingList.Domain.Common;

namespace MyBuyingList.Application.Common.Interfaces.Repositories;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    TEntity? Get(int id);
    Task<TEntity?> GetAsync(int id);
    IEnumerable<TEntity> GetAll();
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Adds a new entity in the database.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>Returns the Id of the added entity.</returns>
    int Add(TEntity entity);
    /// <summary>
    /// Adds a new entity in the database asyncronously.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>Returns the Id of the added entity.</returns>
    Task<int> AddAsync(TEntity entity);
    void AddRange(ICollection<TEntity> entities);
    Task AddRangeAsync(ICollection<TEntity> entities);
    void Delete(TEntity entity);
    Task DeleteAsync(TEntity entity);
    void DeleteRange(ICollection<TEntity> entities);
    Task DeleteRangeAsync(ICollection<TEntity> entities);
    void Edit(TEntity entity);
    Task EditAsync(TEntity entity);
}

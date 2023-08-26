using MyBuyingList.Domain.Common;

namespace MyBuyingList.Application.Common.Interfaces.Repositories;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    TEntity? Get(int id);
    IEnumerable<TEntity> GetAll();
    /// <summary>
    /// Adds a new entity in the database.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>Returns the Id of the added entity.</returns>
    int Add(TEntity entity);
    void AddRange(ICollection<TEntity> entities);
    void Delete(TEntity entity);
    void DeleteRange(ICollection<TEntity> entities);
    void Edit(TEntity entity);
}

namespace MyBuyingList.Application.Common.Interfaces.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    TEntity? Get(int id);
    IEnumerable<TEntity> GetAll();
    void Add(TEntity entity);
    void AddRange(ICollection<TEntity> entities);
    void Delete(TEntity entity);
    void DeleteRange(ICollection<TEntity> entities);
    void Edit(TEntity entity);
}

using MyBuyingList.Application.Common.Errors;
using MyBuyingList.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
    //Result<Unit, DatabaseError> Add(TEntity entity);
    //Result<Unit, DatabaseError> AddRange(ICollection<TEntity> entities);
    //Result<Unit, DatabaseError> Delete(TEntity entity);
    //Result<Unit, DatabaseError> DeleteRange(ICollection<TEntity> entities);
    //Result<Unit, DatabaseError> Edit(TEntity entity);
}

using Microsoft.EntityFrameworkCore;
using MyBuyingList.Application.Common.Errors;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using System.Data;

namespace MyBuyingList.Infrastructure.Repositories;

public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
{
    public ApplicationDbContext _context { get; protected set; }
    protected RepositoryBase(ApplicationDbContext context)
    {
        _context = context;
    }

    public TEntity? Get(int id)
    {
        try
        {
            return _context.Set<TEntity>().Find(id);
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message);
        }        
    }

    public IEnumerable<TEntity> GetAll()
    {
        try
        {
            IEnumerable<TEntity> entities = _context.Set<TEntity>().AsEnumerable();
            return entities.ToList();
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message);
        }        
    }

    public void Add(TEntity entity)
    {
        try
        {
            _context.Set<TEntity>().Add(entity);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message);
        }        
    }

    public void AddRange(ICollection<TEntity> entities)
    {
        try
        {
            _context.Set<TEntity>().AddRange(entities);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message);
        }        
    }

    public void Delete(TEntity entity)
    {
        try
        {
            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message);
        }        
    }

    public void DeleteRange(ICollection<TEntity> entities)
    {
        try
        {
            _context.Set<TEntity>().RemoveRange(entities);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message);
        }        
    }

    public void Edit(TEntity entity)
    {
        try
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message);
        }        
    }
}

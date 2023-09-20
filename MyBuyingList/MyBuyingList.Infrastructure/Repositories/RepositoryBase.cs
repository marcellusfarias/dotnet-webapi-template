using Microsoft.EntityFrameworkCore;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Domain.Common;
using System.Data;
using System.Runtime.CompilerServices;

namespace MyBuyingList.Infrastructure.Repositories;

public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
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
            throw new DatabaseException(ex);
        }        
    }

    public async Task<TEntity?> GetAsync(int id)
    {
        try
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
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
            throw new DatabaseException(ex);
        }        
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            var entities = await _context.Set<TEntity>().ToListAsync();
            return entities;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }

    public int Add(TEntity entity)
    {
        try
        {
            var savedEntity = _context.Set<TEntity>().Add(entity).Entity;
            _context.SaveChanges();

            return savedEntity.Id;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }        
    }

    public async Task<int> AddAsync(TEntity entity)
    {
        try
        {
            var savedEntity = _context.Set<TEntity>().Add(entity).Entity; //if in the future it makes sense to, create a new method that uses AddAsync
            await _context.SaveChangesAsync();

            return savedEntity.Id;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
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
            throw new DatabaseException(ex);
        }        
    }

    public async Task AddRangeAsync(ICollection<TEntity> entities)
    {
        try
        {
            _context.Set<TEntity>().AddRange(entities);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
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
            throw new DatabaseException(ex);
        }        
    }

    public async Task DeleteAsync(TEntity entity)
    {
        try
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
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
            throw new DatabaseException(ex);
        }        
    }

    public async Task DeleteRangeAsync(ICollection<TEntity> entities)
    {
        try
        {
            _context.Set<TEntity>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
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
            throw new DatabaseException(ex);
        }        
    }
    public async Task EditAsync(TEntity entity)
    {
        try
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }

}

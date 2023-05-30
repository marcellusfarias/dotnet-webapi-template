using Microsoft.EntityFrameworkCore;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

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
        return _context.Set<TEntity>().Find(id);
    }

    public IEnumerable<TEntity> GetAll()
    {
        return _context.Set<TEntity>().AsEnumerable();
    }

    public void Add(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
        _context.SaveChanges();
    }

    public void AddRange(ICollection<TEntity> entities)
    {
        _context.Set<TEntity>().AddRange(entities);
        _context.SaveChanges();
    }

    public void Delete(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        _context.SaveChanges();
    }

    public void DeleteRange(ICollection<TEntity> entities)
    {
        _context.Set<TEntity>().RemoveRange(entities);
        _context.SaveChanges();
    }

    public void Edit(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        _context.SaveChanges();
    }
}

﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Domain.Common;

namespace MyBuyingList.Infrastructure.Repositories;

public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    private readonly int _pageSize;

    protected ApplicationDbContext _context { get; set; }

    protected RepositoryBase(ApplicationDbContext context, IOptions<RepositorySettings> options)
    {
        _context = context;
        _pageSize = options.Value.PageSize;
    }

    public async Task<TEntity?> GetAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            return await _context.Set<TEntity>()
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }

    public async Task<List<TEntity>> GetAllAsync(int page, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            var entities = await _context
                .Set<TEntity>()
                .AsNoTracking()
                .Skip((page - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync(cancellationToken);

            return entities;
        }
        catch(OperationCanceledException)
        { 
            throw; 
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }

    public async Task<int> AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            var savedEntity = _context.Set<TEntity>()
                .Add(entity)
                .Entity; //if in the future it makes sense to, create a new method that uses AddAsync
            await _context.SaveChangesAsync(cancellationToken);

            return savedEntity.Id;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }

    public async Task AddRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            _context.Set<TEntity>().AddRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }

    public async Task DeleteRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            _context.Set<TEntity>().RemoveRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }

    public async Task EditAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex);
        }
    }
}

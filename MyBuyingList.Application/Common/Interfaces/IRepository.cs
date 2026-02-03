using MyBuyingList.Domain.Common;

namespace MyBuyingList.Application.Common.Interfaces;

/// <summary>
/// Generic repository interface for basic CRUD operations.
/// </summary>
/// <typeparam name="TEntity">The entity type that extends BaseEntity.</typeparam>
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    /// <summary>
    /// Retrieves an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<TEntity?> GetAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of entities.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of entities for the specified page.</returns>
    Task<List<TEntity>> GetAllAsync(int page, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new entity to the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The identifier of the newly added entity.</returns>
    Task<int> AddAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// Adds multiple entities to the database.
    /// </summary>
    /// <param name="entities">The collection of entities to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an entity from the database.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes multiple entities from the database.
    /// </summary>
    /// <param name="entities">The collection of entities to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">The entity with updated values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task EditAsync(TEntity entity, CancellationToken cancellationToken);
}

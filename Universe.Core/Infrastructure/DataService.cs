using Universe.Core.Data;
using Universe.Core.QueryHandling;
using Universe.Core.Utils;

using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;

namespace Universe.Core.Infrastructure;

/// <inheritdoc/>
public class DataService<TEntity, TKey, TContext> : IDataService<TEntity, TKey, TContext>
	where TEntity : class, IEntity<TKey>
	where TKey : IEquatable<TKey>
	where TContext : IDbContext
{
    private readonly TContext _context;

    public TContext Context => _context;


	public DataService(TContext context)
	{
		VariablesChecker.CheckIsNotNull(context, nameof(context));

		_context = context;
	}

	public virtual async Task<TEntity> Create(TEntity entity, CancellationToken ct = default)
		=> await Create(entity, null, ct);

	public virtual async Task<TEntity> Create(
		TEntity entity,
		Func<TEntity, TContext, CancellationToken, Task>? beforeSave,
		CancellationToken ct = default)
    {
        VariablesChecker.CheckIsNotNull(entity, nameof(entity));

        await _context.Set<TEntity>().AddAsync(entity, ct);

		if (beforeSave is not null)
		{
			await beforeSave.Invoke(entity, _context, ct);
		}

        await _context.SaveChangesAsync(ct);

        return entity;
	}

	public virtual async Task Delete(TKey id, CancellationToken ct = default) => await Delete(id, null, ct);

	public virtual async Task Delete(TKey id, Func<TEntity, TContext, CancellationToken, Task>? beforeSave, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(id, nameof(id));

		var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id != null && x.Id.Equals(id), ct);

		if (entity is not null)
		{
			_context.Set<TEntity>().Remove(entity);

			if (beforeSave is not null)
			{
				await beforeSave.Invoke(entity, _context, ct);
			}

			await _context.SaveChangesAsync(ct);
		}
	}

	public virtual async Task DeleteAll(CancellationToken ct = default) => await DeleteAll(null, ct);


	public virtual async Task DeleteAll(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryHandler, CancellationToken ct = default)
    {
		var set = _context.Set<TEntity>();
		_context.Set<TEntity>().RemoveRange(queryHandler is null ? set : queryHandler(set));
        await _context.SaveChangesAsync(ct);
	}

	public virtual async Task<bool> Exists(TKey id, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(id, nameof(id));

		return await Exists(x => x.Id != null && x.Id.Equals(id), ct);
	}

	public virtual async Task<bool> Exists(Expression<Func<TEntity, bool>> expression, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(expression, nameof(expression));

		return await _context.Set<TEntity>().AnyAsync(expression, ct);
	}

	public virtual async Task<TEntity?> Get(TKey id, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(id, nameof(id));

		return await Get(id, x => x, ct);
	}

	public virtual async Task<TEntity?> Get(TKey id, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryHandler, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(id, nameof(id));

		var set = _context.Set<TEntity>().AsNoTracking();
		return await (queryHandler is null ? set : queryHandler(set)).SingleOrDefaultAsync(x => x.Id != null && x.Id.Equals(id), ct);
	}

	public virtual async Task<TEntity?> Get(Expression<Func<TEntity, bool>> expression, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(expression, nameof(expression));

		return await _context.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(expression, ct);
	}

	public virtual async Task<IEnumerable<TEntity>> GetAll(CancellationToken ct = default) => await GetAll(x => x, ct);

	public virtual async Task<IEnumerable<TEntity>> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryHandler, CancellationToken ct = default)
		=> await GetAll(null, queryHandler, ct);

	public virtual async Task<IEnumerable<TEntity>> GetAll(QueryCommand? command, CancellationToken ct = default)
		=> await GetAll(command, x => x, ct);

	public virtual async Task<IEnumerable<TEntity>> GetAll(
        QueryCommand? command,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryHandler,
        CancellationToken ct = default)
	{
		var set = _context.Set<TEntity>().AsNoTracking();
		var query = queryHandler is null ? set : queryHandler(set);

        if (command is null) return await query.ToListAsync(ct);

		query = QueryCommandApplier.ApplyExpand(query, command);
		query = QueryCommandApplier.ApplyFiltering(query, command);
		query = QueryCommandApplier.ApplySorting(query, command);

		if (command.Skip is not null)
			query = query.Skip(command.Skip.Value);

		if (command.Take is not null)
			query = query.Take(command.Take.Value);

		return await query.ToListAsync(ct);
	}

	public virtual async Task<int> GetTotal(CancellationToken ct = default) => await GetTotal(x => x, ct);

	public virtual async Task<int> GetTotal(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryHandler, CancellationToken ct = default)
		=> await GetTotal(null, queryHandler, ct);

	public virtual async Task<int> GetTotal(QueryCommand? command, CancellationToken ct = default)
		=> await GetTotal(command, x => x, ct);

	public virtual async Task<int> GetTotal(
		QueryCommand? command,
		Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryHandler,
		CancellationToken ct = default)
	{
		var set = _context.Set<TEntity>().AsNoTracking();
		var query = queryHandler is null ? set : queryHandler(set);

		if (command is null) return await query.CountAsync(ct);

		query = QueryCommandApplier.ApplyExpand(query, command);
		query = QueryCommandApplier.ApplyFiltering(query, command);
		query = QueryCommandApplier.ApplySorting(query, command);

		return await query.CountAsync(ct);
	}

	public virtual async Task<TEntity> Update(TEntity entity, CancellationToken ct = default)
		=> await Update(entity, null, ct);

	public virtual async Task<TEntity> Update(TEntity entity, Func<TEntity, TContext, CancellationToken, Task>? beforeSave, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(entity, nameof(entity));

		_context.Entry(entity).State = EntityState.Modified;

		if (beforeSave is not null)
		{
			await beforeSave.Invoke(entity, _context, ct);
		}

		await _context.SaveChangesAsync(ct);

		return entity;
	}	
}

/// <inheritdoc/>
public class DataService<TEntity, TKey> : DataService<TEntity, TKey, IDbContext>, IDataService<TEntity, TKey>
	where TEntity : class, IEntity<TKey>
	where TKey : IEquatable<TKey>
{
    public DataService(IDbContext context) : base(context)
    {
    }
}

public class DataService<TEntity> : DataService<TEntity, int>, IDataService<TEntity>
	where TEntity : class, IEntity
{
	public DataService(IDbContext context) : base(context)
	{
	}
}

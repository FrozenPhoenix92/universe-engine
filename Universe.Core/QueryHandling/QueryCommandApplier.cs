using Universe.Core.Extensions;

using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;
using System.Reflection;

namespace Universe.Core.QueryHandling;

public static class QueryCommandApplier
{
	public static IQueryable<TEntity> ApplyExpand<TEntity>(IQueryable<TEntity> query, QueryCommand command) where TEntity : class
	{
		if (command.ExpandedFields is null || !command.ExpandedFields.Any()) return query;

		foreach (var expandedField in command.ExpandedFields)
		{
			query.Include(expandedField);
		}

		return query;
	}

	public static IQueryable<TEntity> ApplyFiltering<TEntity>(IQueryable<TEntity> query, QueryCommand command) where TEntity : class
	{
		if (command.Filters is null || !command.Filters.Any()) return query;

		foreach (var groupedFilters in command.Filters.GroupBy(a => a.PropertyName!.ToLowerInvariant()))
		{
			Expression<Func<TEntity, bool>>? orExpression = null;
			foreach (var filterInfo in groupedFilters)
			{
				CheckPropertyChain<TEntity>(filterInfo.PropertyName!);

				var expr = FilterHandlersProvider.ProvideFilter<TEntity>(filterInfo);
				if (expr is not null)
					orExpression = orExpression is null ? expr : orExpression.Or(expr);
			}

			if (orExpression is not null)
			query = query.Where(orExpression);
		}

		return query;
	}

	public static IQueryable<TEntity> ApplySorting<TEntity>(IQueryable<TEntity> query, QueryCommand command) where TEntity : class
	{
		if (string.IsNullOrEmpty(command.SortingField)) return query;

		CheckPropertyChain<TEntity>(command.SortingField);
		var sortingDirection = command.SortingDirection ?? OrderDirection.Asc;

		var param = Expression.Parameter(typeof(TEntity), "item");
		var body = command.SortingField.Split('.').Aggregate<string, Expression>(param, Expression.Property);
		var lambda = Expression.Lambda(body, param);

		var method = typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
			.Where(a => a.Name == $"OrderBy{(sortingDirection == OrderDirection.Asc ? string.Empty : "Descending")}")
			.Single(a => a.GetParameters().Length == 2);
		method = method.MakeGenericMethod(typeof(TEntity), body.Type);

		return (IQueryable<TEntity>?)method?.Invoke(method, new object[] { query, lambda }) ?? query;
	}


	private static void CheckPropertyChain<TEntity>(string propertyChain)
	{
		var propertyName = propertyChain?.Split('.') ?? new string[0];
		PropertyInfo? entityPropertyInfo = null;
		var type = typeof(TEntity);

		foreach (var part in propertyName)
		{
			entityPropertyInfo = type.GetProperty(part, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if (entityPropertyInfo is not null)
				type = entityPropertyInfo.PropertyType;
			else
				break;
		}

		if (entityPropertyInfo is null)
			throw new ArgumentException($"”казанное в запросе поле '{propertyChain}' не существует в типе '{typeof(TEntity).FullName}'.");
	}
}
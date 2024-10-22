using Universe.Core.QueryHandling.Filters;

using System.Linq.Expressions;
using System.Net;
using System.Reflection;

namespace Universe.Core.QueryHandling.Handlers;

public class StringFilterHandler : FilterHandlerBase
{
	private readonly StringFilter _filter;

	public StringFilterHandler(StringFilter filter)
	{
		_filter = filter;
	}

	public override Expression<Func<TEntity, bool>> Handle<TEntity>()
	{
		var item = Expression.Parameter(typeof(TEntity), "item");
		var property = GetProperty(item, _filter.PropertyName!);

		if (property.Type == typeof(Guid))
		{
			return HandleGuid<TEntity>(item, property);
		}

		if (property.Type == typeof(Guid?))
		{
			return HandleGuid<TEntity>(item, Expression.Property(property, "Value"));
		}

		if (property.Type.IsEnum)
		{
			var mi = typeof(Enum).GetMethods()
				.Where(x => x.Name == "Parse" && x.IsGenericMethod &&
					x.GetParameters().First().ParameterType == typeof(string) && x.GetParameters().Last().ParameterType == typeof(bool))
				.Single();

			var constantValue = Expression.Convert(
				Expression.Constant(
					mi.MakeGenericMethod(property.Type).Invoke(null, [(string)(object)_filter.Value!, true])),
				property.Type);

			return Expression.Lambda<Func<TEntity, bool>>(
				_filter.MatchMode == StringFilterMatchMode.NotEquals
					? Expression.NotEqual(property, constantValue)
					: Expression.Equal(property, constantValue),
				item);
		}

		var value = Expression.Constant(WebUtility.UrlDecode(_filter.Value?.ToUpperInvariant()));
		property = Expression.Call(property, "ToUpper", null);

		// We have made the decision that BBWT3 will, by default, convert all searches to be case-insensitive.
		// We are not currently supporting the option for case-sensitive searches, but that feature could be added at a later point.
		string methodName;
		var isNotNullExpression = Expression.NotEqual(property, Expression.Constant(null));
		methodName = _filter.MatchMode switch
		{
			StringFilterMatchMode.Contains => "Contains",
			StringFilterMatchMode.NotContains => "Contains",
			StringFilterMatchMode.StartsWith => "StartsWith",
			StringFilterMatchMode.EndsWith => "EndsWith",
			_ => "Equals"
		};
		var method = typeof(string).GetMethod(methodName, new Type[] { typeof(string) });
		var body = Expression.Call(property, method, value);
		var resultExpression = Expression.AndAlso(
			isNotNullExpression,
			_filter.MatchMode != StringFilterMatchMode.NotContains && _filter.MatchMode != StringFilterMatchMode.NotEquals
				? body
				: Expression.Not(body));
		return Expression.Lambda<Func<TEntity, bool>>(resultExpression, item);
	}

	private Expression<Func<TEntity, bool>> HandleGuid<TEntity>(ParameterExpression item, Expression property)
	{
		var value = Expression.Constant(Guid.Parse(_filter.Value!));
		var method = typeof(Guid).GetMethod("Equals", new Type[] { typeof(Guid) });
		var body = Expression.Call(property, method!, value);

		return Expression.Lambda<Func<TEntity, bool>>(_filter.MatchMode != StringFilterMatchMode.NotEquals ? body : Expression.Not(body), item);
	}
}

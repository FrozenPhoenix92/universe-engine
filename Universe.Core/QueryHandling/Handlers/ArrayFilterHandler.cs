using Universe.Core.QueryHandling.Filters;

using System.Linq.Expressions;
using System.Reflection;

namespace Universe.Core.QueryHandling.Handlers;

public class ArrayFilterHandler<T> : ValueFilterHandlerBase
{
	private readonly ArrayFilter<T> _filter;

	protected ArrayFilterHandler(ArrayFilter<T> filter) => _filter = filter;

	public override Expression<Func<TEntity, bool>> Handle<TEntity>()
	{
		var item = Expression.Parameter(typeof(TEntity), "item");
		var property = GetProperty(item, _filter.PropertyName!);

		NewArrayExpression array;
		if (property.Type.IsEnum)
		{
			var mi = typeof(Enum).GetMethods()
				.Where(x => x.Name == "Parse" && x.IsGenericMethod &&
					x.GetParameters().First().ParameterType == typeof(string) && x.GetParameters().Last().ParameterType == typeof(bool))
				.Single();

			array = Expression.NewArrayInit(
				property.Type,
				_filter.Value!.Select(
					x => Expression.Convert(
						Expression.Constant(
							mi.MakeGenericMethod(property.Type).Invoke(null, [(string)(object)x!, true])),
						property.Type)));
		}
		else
		{
			array = Expression.NewArrayInit(property.Type, _filter.Value!.Select(x => Expression.Convert(Expression.Constant(x), property.Type)));
		}
		var contains = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
			.Single(x => x.Name == "Contains" && x.GetParameters().Length == 2)
			.MakeGenericMethod(property.Type);
		var body = Expression.Call(contains, array, property);
		return Expression.Lambda<Func<TEntity, bool>>(body, item);
	}
}

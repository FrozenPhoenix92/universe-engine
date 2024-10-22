using System.Linq.Expressions;
using System.Reflection;

using Universe.Core.Exceptions;
using Universe.Core.QueryHandling.Filters;
using Universe.Core.QueryHandling.Handlers;

namespace Universe.Core.QueryHandling;

public static class FilterHandlersProvider
{
    public static Expression<Func<TEntity, bool>> ProvideFilter<TEntity>(FilterInfoBase filter)
    {
        var attribute = filter.GetType().GetTypeInfo().GetCustomAttribute<RelatedHandlerAttribute>();

        if (attribute is null)
            throw new ConflictException($"Для фильтра типа '{filter.GetType().FullName}' не указан обработчик.");

        var handler = Activator.CreateInstance(attribute.HandlerType, filter) as FilterHandlerBase;

        if (handler is null)
            throw new ConflictException($"Для фильтра типа '{filter.GetType().FullName}' не удалось создать обработчик.");

        return handler.Handle<TEntity>();
    }
}

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
            throw new ConflictException($"��� ������� ���� '{filter.GetType().FullName}' �� ������ ����������.");

        var handler = Activator.CreateInstance(attribute.HandlerType, filter) as FilterHandlerBase;

        if (handler is null)
            throw new ConflictException($"��� ������� ���� '{filter.GetType().FullName}' �� ������� ������� ����������.");

        return handler.Handle<TEntity>();
    }
}

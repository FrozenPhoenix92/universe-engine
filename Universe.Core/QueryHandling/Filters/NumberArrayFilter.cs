using Universe.Core.QueryHandling.Handlers;

namespace Universe.Core.QueryHandling.Filters;

[RelatedHandler(typeof(NumberArrayFilterHandler))]
public class NumberArrayFilter : ArrayFilter<int>
{
}

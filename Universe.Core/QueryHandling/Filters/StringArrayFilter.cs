using Universe.Core.QueryHandling.Handlers;

namespace Universe.Core.QueryHandling.Filters;

[RelatedHandler(typeof(StringArrayFilterHandler))]
public class StringArrayFilter : ArrayFilter<string>
{
}

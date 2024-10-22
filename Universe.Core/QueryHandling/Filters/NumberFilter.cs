using Universe.Core.QueryHandling.Handlers;

namespace Universe.Core.QueryHandling.Filters;

[RelatedHandler(typeof(NumberFilterHandler))]
public class NumberFilter : CountableFilterBase<double>
{
}

[RelatedHandler(typeof(CountableBetweenFilterHandler<double>))]
public class NumberBetweenFilter : CountableBetweenFilterBase<double>
{
}

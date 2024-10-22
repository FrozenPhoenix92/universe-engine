using Universe.Core.QueryHandling.Handlers;

namespace Universe.Core.QueryHandling.Filters;

[RelatedHandler(typeof(DateFilterHandler))]
public class DateFilter : CountableFilterBase<DateTime>
{
}

[RelatedHandler(typeof(CountableBetweenFilterHandler<DateTime>))]
public class DateBetweenFilter : CountableBetweenFilterBase<DateTime>
{
}

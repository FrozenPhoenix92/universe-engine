using Universe.Core.QueryHandling.Filters;

namespace Universe.Core.QueryHandling.Handlers;

public class NumberFilterHandler : CountableFilterHandler<double>
{
	public NumberFilterHandler(NumberFilter filter) : base(filter)
	{
	}
}

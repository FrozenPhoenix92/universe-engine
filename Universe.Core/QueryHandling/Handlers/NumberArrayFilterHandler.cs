using Universe.Core.QueryHandling.Filters;

namespace Universe.Core.QueryHandling.Handlers
{
	public class NumberArrayFilterHandler : ArrayFilterHandler<int>
	{
		public NumberArrayFilterHandler(ArrayFilter<int> filter) : base(filter)
		{
		}
	}
}

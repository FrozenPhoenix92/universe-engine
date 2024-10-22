using Universe.Core.QueryHandling.Filters;

namespace Universe.Core.QueryHandling.Handlers;

	public class StringArrayFilterHandler : ArrayFilterHandler<string>
{
    public StringArrayFilterHandler(ArrayFilter<string> filter) : base(filter)
    {
    }
}

using Universe.Core.QueryHandling.Handlers;

namespace Universe.Core.QueryHandling.Filters;

/// <summary>
/// Ternary logic:
/// true => Where(item => item.Property)
/// false =>  Where(item => !item.Property)
/// null => Where(item => true) 
/// </summary>
[RelatedHandler(typeof(BooleanFilterHandler))]
public class BooleanFilter : FilterInfoBase<bool?>
{
}

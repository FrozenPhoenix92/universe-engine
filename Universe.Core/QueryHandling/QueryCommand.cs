using Universe.Core.QueryHandling.Filters;

namespace Universe.Core.QueryHandling;

/// <summary>
/// Представляет команду, определяющую параметры объекта запроса.
/// </summary>
public class QueryCommand
{
	/// <summary>
	/// Названия полей со сложным типом данных, которые необходимо дополнительно подгрузить.
	/// </summary>
	public string[] ExpandedFields { get; set; } = Array.Empty<string>();

	/// <summary>
	/// Список фильтров.
	/// </summary>
	public IList<FilterInfoBase> Filters { get; set; } = new List<FilterInfoBase>();

	/// <summary>
	/// Пропускаемое количество элементов.
	/// </summary>
	public int? Skip { get; set; }

	/// <summary>
	/// Поле, по которому проводится сортировка.
	/// </summary>
	public string? SortingField { get; set; }

	/// <summary>
	/// Направление сортировки.
	/// </summary>
	public OrderDirection? SortingDirection { get; set; }

	/// <summary>
	/// Максимальное количество элементов.
	/// </summary>
	public int? Take { get; set; }
}

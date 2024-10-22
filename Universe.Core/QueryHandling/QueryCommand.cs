using Universe.Core.QueryHandling.Filters;

namespace Universe.Core.QueryHandling;

/// <summary>
/// ������������ �������, ������������ ��������� ������� �������.
/// </summary>
public class QueryCommand
{
	/// <summary>
	/// �������� ����� �� ������� ����� ������, ������� ���������� ������������� ����������.
	/// </summary>
	public string[] ExpandedFields { get; set; } = Array.Empty<string>();

	/// <summary>
	/// ������ ��������.
	/// </summary>
	public IList<FilterInfoBase> Filters { get; set; } = new List<FilterInfoBase>();

	/// <summary>
	/// ������������ ���������� ���������.
	/// </summary>
	public int? Skip { get; set; }

	/// <summary>
	/// ����, �� �������� ���������� ����������.
	/// </summary>
	public string? SortingField { get; set; }

	/// <summary>
	/// ����������� ����������.
	/// </summary>
	public OrderDirection? SortingDirection { get; set; }

	/// <summary>
	/// ������������ ���������� ���������.
	/// </summary>
	public int? Take { get; set; }
}

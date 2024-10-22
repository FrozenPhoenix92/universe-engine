namespace Universe.Core.Configuration;

/// <summary>
/// ������������, ������������ ��� ���� ������.
/// </summary>
public enum DatabaseType
{
	MsSql = 1,
	MySql = 2,
	PostgreSql = 3
}

/// <summary>
/// ������������ ����� ��������, ������������ ������ ���������� � ����� ������.
/// </summary>
public class DatabaseConnectionSettings
{
	/// <summary>
	/// ������ �����������.
	/// </summary>
	public string? ConnectionString { get; set; }

	/// <summary>
	/// ��� ���� ������.
	/// </summary>
	public DatabaseType? DatabaseType { get; set; }

	/// <summary>
	/// ������������ ���������� �������� ������� ����������.
	/// </summary>
	public int MaxRetryCount { get; set; } = 10;

	/// <summary>
	/// ������������ �������� � �������� ����� ��������� ����������.
	/// </summary>
	public int MaxRetryDelay { get; set; } = 30;


	/// <summary>
	/// �������������� ���� SQL ������.
	/// </summary>
	public ICollection<int> ErrorNumbersToAdd { get; set; } = new List<int>();
}

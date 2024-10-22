namespace Universe.Core.Configuration;

/// <summary>
/// Перечисление, определяющее тип базы данных.
/// </summary>
public enum DatabaseType
{
	MsSql = 1,
	MySql = 2,
	PostgreSql = 3
}

/// <summary>
/// Представляет набор настроек, определяющих работу соединения с базой данных.
/// </summary>
public class DatabaseConnectionSettings
{
	/// <summary>
	/// Строка подключения.
	/// </summary>
	public string? ConnectionString { get; set; }

	/// <summary>
	/// Тип базы данных.
	/// </summary>
	public DatabaseType? DatabaseType { get; set; }

	/// <summary>
	/// Максимальное количество повторов попытки соединения.
	/// </summary>
	public int MaxRetryCount { get; set; } = 10;

	/// <summary>
	/// Максимальная задержка в секундах между попытками соединения.
	/// </summary>
	public int MaxRetryDelay { get; set; } = 30;


	/// <summary>
	/// Дополнительные коды SQL ошибок.
	/// </summary>
	public ICollection<int> ErrorNumbersToAdd { get; set; } = new List<int>();
}

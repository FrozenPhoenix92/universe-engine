namespace BBWM.Messages;

/// <summary>
/// Определяет набор настроек для механизма отправки электронных сообщений.
/// </summary>
public class EmailMessagingSettings
{
	/// <summary>
	/// Включить ли SSL.
	/// </summary>
	public bool EnableSsl { get; set; }

	/// <summary>
	/// Адрес отправки.
	/// </summary>
	public string? FromAddress { get; set; }

	/// <summary>
	/// Пароль для авторизации в почтовом сервере.
	/// </summary>
	public string? Password { get; set; }

	/// <summary>
	/// SMTP порт сервера.
	/// </summary>
	public int? Port { get; set; }

	/// <summary>
	/// SMTP адрес сервера.
	/// </summary>
	public string? Smtp { get; set; }

	/// <summary>
	/// Логин для авторизации в почтовом сервере.
	/// </summary>
	public string? UserName { get; set; }
}

namespace BBWM.Messages;

/// <summary>
/// ���������� ����� �������� ��� ��������� �������� ����������� ���������.
/// </summary>
public class EmailMessagingSettings
{
	/// <summary>
	/// �������� �� SSL.
	/// </summary>
	public bool EnableSsl { get; set; }

	/// <summary>
	/// ����� ��������.
	/// </summary>
	public string? FromAddress { get; set; }

	/// <summary>
	/// ������ ��� ����������� � �������� �������.
	/// </summary>
	public string? Password { get; set; }

	/// <summary>
	/// SMTP ���� �������.
	/// </summary>
	public int? Port { get; set; }

	/// <summary>
	/// SMTP ����� �������.
	/// </summary>
	public string? Smtp { get; set; }

	/// <summary>
	/// ����� ��� ����������� � �������� �������.
	/// </summary>
	public string? UserName { get; set; }
}

using Universe.Core.Exceptions;

namespace Universe.Core.Membership.Configuration;

/// <summary>
/// ������������ ����� ��������, ������������ ����������� ����� cookie.
/// </summary>
public class CookieAuthenticationSettings
{
	private string _cookieName = ".AspNetCore.Cookies";
	private int _expireTime = 30;


	/// <summary>
	/// ����� ������� ���� ����� � API.
	/// </summary>
	public string[] ApiPath { get; set; } = Array.Empty<string>();

	/// <summary>
	/// <para>��� ���������������� cookie.</para>
	/// <para>�� ��������� ".AspNetCore.Cookies".</para>
	/// </summary>
	public string CookieName
	{
		get => _cookieName;
		set => _cookieName = !string.IsNullOrWhiteSpace(value)
			? value
			: ".AspNetCore.Cookies";
	}

	/// <summary>
	/// <para>����� � �������, ������� � ������� �����������, � ������� �������� �������, ���������� � cookie, ��������� ��������.</para>
	/// <para>�� ��������� 30 �����.</para>
	/// </summary>
	public int ExpireTime
	{
		get => _expireTime;
		set => _expireTime = value > 0
			? value
			: throw new DataException("����� � �������, ������� � ������� �����������, � ������� �������� �������, " +
				"���������� � cookie, ��������� ��������, ������ ���� ������ 0.");
	}

	/// <summary>
	/// ���� � �������� ������.
	/// </summary>
	public string? LoginPath { get; set; }
}

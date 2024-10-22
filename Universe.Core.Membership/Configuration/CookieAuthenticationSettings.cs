using Universe.Core.Exceptions;

namespace Universe.Core.Membership.Configuration;

/// <summary>
/// ѕредставл€ет набор настроек, определ€ющий авторизацию через cookie.
/// </summary>
public class CookieAuthenticationSettings
{
	private string _cookieName = ".AspNetCore.Cookies";
	private int _expireTime = 30;


	/// <summary>
	/// ќбщий префикс всех путей к API.
	/// </summary>
	public string[] ApiPath { get; set; } = Array.Empty<string>();

	/// <summary>
	/// <para>»м€ авторизационного cookie.</para>
	/// <para>ѕо умолчанию ".AspNetCore.Cookies".</para>
	/// </summary>
	public string CookieName
	{
		get => _cookieName;
		set => _cookieName = !string.IsNullOrWhiteSpace(value)
			? value
			: ".AspNetCore.Cookies";
	}

	/// <summary>
	/// <para>¬рем€ в минутах, начина€ с момента авторизации, в течение которого пропуск, хран€щийс€ в cookie, считаетс€ валидным.</para>
	/// <para>ѕо умолчанию 30 минут.</para>
	/// </summary>
	public int ExpireTime
	{
		get => _expireTime;
		set => _expireTime = value > 0
			? value
			: throw new DataException("¬рем€ в минутах, начина€ с момента авторизации, в течение которого пропуск, " +
				"хран€щийс€ в cookie, считаетс€ валидным, должно быть больше 0.");
	}

	/// <summary>
	/// ѕуть к странице логина.
	/// </summary>
	public string? LoginPath { get; set; }
}

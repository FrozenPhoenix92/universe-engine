using Universe.Core.Exceptions;

namespace Universe.Web.Configuration;

public class ClientAppJwtSettings
{
	private int _expireTime = 180;


	public string Audience { get; set; } = string.Empty;

	/// <summary>
	/// <para>Время в минутах, начиная с момента авторизации, в течение которого пропуск, хранящийся в cookie, считается валидным.</para>
	/// <para>По умолчанию 30 минут.</para>
	/// </summary>
	public int ExpireTime
	{
		get => _expireTime;
		set => _expireTime = value > 0
			? value
			: throw new DataException("Время в минутах, начиная с момента авторизации, в течение которого пропуск, " +
				"хранящийся в cookie, считается валидным, должно быть больше 0.");
	}

	public string[] Issuers { get; set; } = Array.Empty<string>();

	public string SigningKey { get; set; } = string.Empty;
}

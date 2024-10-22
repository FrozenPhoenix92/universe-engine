using Universe.Core.AppConfiguration;
using Universe.Core.Exceptions;
using Universe.Core.Membership.Model;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System.Reflection;

namespace Universe.Core.Membership.Configuration;

/// <summary>
/// Представляет набор настроек, определяющих правила авторизации.
/// </summary>
public class AuthorizationSettings : IAppSettingsOnChangeCallback
{
	private int _lockoutTimeSpan = 300;
	private int _maxFailedAccessAttempts = 3;


	/// <summary>
	/// <para>Длительность блокировки пользователя в секундах.</para>
	/// <para>По умолчанию 300 (5 минут).</para>
	/// </summary>
	public int LockoutTimeSpan
	{
		get => _lockoutTimeSpan;
		set => _lockoutTimeSpan = value >= 0
			? value
			: throw new DataException("Длительность блокировки пользователя в секундах должна быть больше либо равно 0.");
	}

	/// <summary>
	/// <para>Максимально допустимое количество неудачных попыток авторизации,
	/// превышение которого ведёт к блокировке пользователя.</para>
	/// <para>По умолчанию 3.</para>
	/// </summary>
	public int MaxFailedAccessAttempts
	{
		get => _maxFailedAccessAttempts;
		set => _maxFailedAccessAttempts = value >= 0
			? value
			: throw new DataException("Максимально допустимое количество неудачных попыток авторизации должно быть больше либо равно 0.");
	}

	/// <summary>
	/// <para>Определяет, включено ли ограничение на количество неудачныз попыток авторизации.</para>
	/// <para>По умолчанию true.</para>
	/// </summary>
	public bool LimitFailedAccessAttempts { get; set; } = true;


	public Task OnChange(IServiceProvider serviceProvider, CancellationToken ct = default)
	{
		var identityOptions = serviceProvider.GetRequiredService<IOptions<IdentityOptions>>()?.Value;
		if (identityOptions is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(IOptions<IdentityOptions>));

		identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(LockoutTimeSpan);
		identityOptions.Lockout.MaxFailedAccessAttempts = MaxFailedAccessAttempts;

		return Task.CompletedTask;
	}
}

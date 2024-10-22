using Universe.Core.AppConfiguration;
using Universe.Core.Exceptions;
using Universe.Core.Membership.Model;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Options;

using System.Reflection;

namespace Universe.Core.Membership.Configuration;

/// <summary>
/// Представляет набор настроек, определяющих требования к строгости пароля.
/// </summary>
public class PasswordSettings : IAppSettingsOnChangeCallback
{
	private int _requiredLength = 6;
	private int _requiredUniqueChars = 1;


	/// <summary>
	/// <para>Определяет, должен ли пароль обязательно содержать цифру.</para>
	/// <para>По умолчанию false.</para>
	/// </summary>
	public bool RequireDigit { get; set; } = false;

	/// <summary>
	/// <para>Определяет минимальную длину пароля.</para>
	/// <para>По умолчанию 6.</para>
	/// </summary>
	public int RequiredLength 
	{
		get => _requiredLength;
		set => _requiredLength = value >= 4
			? value
			: throw new DataException("Минимальная длина пароля должна быть больше либо равна 4.");
	}

	/// <summary>
	/// <para>Определяет, должен ли пароль обязательно содержать строчную букву.</para>
	/// <para>По умолчанию false.</para>
	/// </summary>
	public bool RequireLowercase { get; set; } = false;

	/// <summary>
	/// <para>Определяет, должен ли пароль обязательно содержать символ, отличный от цифры или буквы.</para>
	/// <para>По умолчанию false.</para>
	/// </summary>
	public bool RequireNonAlphanumeric { get; set; } = false;

	/// <summary>
	/// <para>Определяет минимальное количество отличающихся друг от друга символов.</para>
	/// <para>По умолчанию 1.</para>
	/// </summary>
	public int RequiredUniqueChars
	{
		get => _requiredUniqueChars;
		set => _requiredUniqueChars = value >= 1
			? value
			: throw new DataException("Количество отличающихся друг от друга символов должно быть больше либо равно 1.");
	}

	/// <summary>
	/// <para>Определяет, должен ли пароль обязательно содержать заглавную букву.</para>
	/// <para>По умолчанию false.</para>
	/// </summary>
	public bool RequireUppercase { get; set; } = false;


	public Task OnChange(IServiceProvider serviceProvider, CancellationToken ct = default)
	{
		var identityOptions = serviceProvider.GetRequiredService<IOptions<IdentityOptions>>()?.Value;
		if (identityOptions is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(IOptions<IdentityOptions>));

		identityOptions.Password.RequireDigit = RequireDigit;
		identityOptions.Password.RequiredLength = RequiredLength;
		identityOptions.Password.RequireLowercase = RequireLowercase;
		identityOptions.Password.RequireNonAlphanumeric = RequireNonAlphanumeric;
		identityOptions.Password.RequiredUniqueChars = RequiredUniqueChars;
		identityOptions.Password.RequireUppercase = RequireUppercase;

		return Task.CompletedTask;
	}
}

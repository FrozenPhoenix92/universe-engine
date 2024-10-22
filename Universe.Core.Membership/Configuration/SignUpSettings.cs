using Universe.Core.AppConfiguration;
using Universe.Core.Exceptions;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Options;

using System.Reflection;

using UniverseDataException = Universe.Core.Exceptions.DataException;

namespace Universe.Core.Membership.Configuration;

/// <summary>
///  Представляет набор настроек, определяющих требования к регистрации.
/// </summary>
public class SignUpSettings : IAppSettingsOnChangeCallback
{
	private int _changePhoneNumberTokenExpireTimespan = 120;
	private string _confirmEmailCallbackUrl = "/email-confirmation";
	private int _confirmEmailTokenExpireTimespan = 10080;
	private string _inviteCallbackUrl = "/accept-invitation";
	private int _inviteTokenExpireTimespan = 10080;
	private string _resetPasswordCallbackUrl = "/reset-password";
	private int _resetPasswordTokenExpireTimespan = 1440;


	/// <summary>
	/// <para>Определяет, должен ли пользователь автоматически входить в систему после прохождения регистрации.</para>
	/// <para>По умолчанию false.</para>
	/// </summary>
	public bool AutoSignIn { get; set; }	
	
	/// <summary>
	/// <para>Время жизни токена в минутах, исползуемого для валидации процесса подтверждения номера телефона.</para>
	/// <para>По умолчанию 120 (2 минуты).</para>
	/// </summary>
	public int ChangePhoneNumberTokenExpireTimespan
	{
		get => _changePhoneNumberTokenExpireTimespan;
		set => _changePhoneNumberTokenExpireTimespan = value >= 1
			? value
			: throw new UniverseDataException("Время жизни токена в минутах, исползуемого для валидации процесса подтверждения " +
				"номера телефона, должно быть больше либо равно 1.");
	}

	/// <summary>
	/// <para>Относительный путь обратной ссылки на страницу подтверждения адреса электронной почты.</para>
	/// <para>По умолчанию "/email-confirmation".</para>
	/// </summary>
	public string ConfirmEmailCallbackUrl
	{
		get => _confirmEmailCallbackUrl;
		set => _confirmEmailCallbackUrl = !string.IsNullOrWhiteSpace(value)
			? value
			: throw new UniverseDataException("Относительный путь обратной ссылки на страницу подтверждения адреса электронной почты не должен быть пустым.");
	}

	/// <summary>
	/// <para>Время жизни токена в минутах, исползуемого для валидации процесса подтверждения адреса электронной почты.</para>
	/// <para>По умолчанию 10080 (1 неделя).</para>
	/// </summary>
	public int ConfirmEmailTokenExpireTimespan
	{
		get => _confirmEmailTokenExpireTimespan;
		set => _confirmEmailTokenExpireTimespan = value >= 1
			? value
			: throw new UniverseDataException("Время жизни токена в минутах, исползуемого для валидации процесса подтверждения " +
				"адреса электронной почты, должно быть больше либо равно 1.");
	}

	/// <summary>
	/// <para>Относительный путь обратной ссылки на страницу принятия приглашения.</para>
	/// <para>По умолчанию "/accept-invitation".</para>
	/// </summary>
	public string InviteCallbackUrl
	{
		get => _inviteCallbackUrl;
		set => _inviteCallbackUrl = !string.IsNullOrWhiteSpace(value)
			? value
			: throw new UniverseDataException("Относительный путь обратной ссылки на страницу принятия приглашения не должен быть пустым.");
	}

	/// <summary>
	/// <para>Время жизни токена в минутах, исползуемого для валидации процесса подтверждения приглашения новым пользователем.</para>
	/// <para>По умолчанию 10080 (1 неделя).</para>
	/// </summary>
	public int InviteTokenExpireTimespan
	{
		get => _inviteTokenExpireTimespan;
		set => _inviteTokenExpireTimespan = value >= 1
			? value
			: throw new UniverseDataException("Время жизни токена в минутах, исползуемого для валидации процесса подтверждения " +
				"приглашения новым пользователем, должно быть больше либо равно 1.");
	}

	/// <summary>
	/// <para>Необходимо ли подтверждение аккаунта зарегистрировавшегося пользователя администратором
	/// для активации возможности авторизщации.</para>
	/// </para>По умолчанию false.</para>
	/// </summary>
	public bool RequireApproval { get; set; } = false;

	/// <summary>
	/// <para>Является ли подтверждение адреса электронной почты обязательным условием для успешной авторизации.</para>
	/// </para>По умолчанию true.</para>
	/// </summary>
	public bool RequireConfirmedEmail { get; set; } = true;

	/// <summary>
	/// <para>Относительный путь обратной ссылки на страницу изменения пароля.</para>
	/// <para>По умолчанию "/reset-password".</para>
	/// </summary>
	public string ResetPasswordCallbackUrl
	{
		get => _resetPasswordCallbackUrl;
		set => _resetPasswordCallbackUrl = !string.IsNullOrWhiteSpace(value)
			? value
			: throw new UniverseDataException("Относительный путь обратной ссылки на страницу изменения пароля не должен быть пустым.");
	}

	/// <summary>
	/// <para>Время жизни токена в минутах, исползуемого для валидации процесса восстановления пароля.</para>
	/// <para>По умолчанию 1440 (1 день).</para>
	/// </summary>
	public int ResetPasswordTokenExpireTimespan
	{
		get => _resetPasswordTokenExpireTimespan;
		set => _resetPasswordTokenExpireTimespan = value >= 1
			? value
			: throw new UniverseDataException("Время жизни токена в минутах, " +
				"исползуемого для валидации процесса восстановления пароля, должно быть больше либо равно 1.");
	}

	/// <summary>
	/// <para>Доступна ли самостоятельная регистрация новых ползователей.</para>
	/// <para>По умолчанию true.</para>
	/// </summary>
	public bool SelfSignUpEnabled { get; set; } = true;

	/// <summary>
	/// Роль, получаемая пользователем после самостоятельной регистрации.
	/// </summary>
	public string? SelfSignUpRole { get; set; }


	public Task OnChange(IServiceProvider serviceProvider, CancellationToken ct = default)
	{
		var identityOptions = serviceProvider.GetRequiredService<IOptions<IdentityOptions>>()?.Value;
		if (identityOptions is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(IOptions<IdentityOptions>));

		identityOptions.SignIn.RequireConfirmedEmail = RequireConfirmedEmail;
		identityOptions.SignIn.RequireConfirmedAccount = RequireApproval;

		return Task.CompletedTask;
	}
}

using Universe.Core.Exceptions;

namespace Universe.Web.Configuration;

public class ClientApplicationSettings
{
	private string _applicationCode { get; set; } = string.Empty;
	private string _applicationName { get; set; } = string.Empty;
	private string _completeRegistrationCallbackUrl = "/sign-up";
	private string _confirmEmailCallbackUrl = "/email-confirmation";
	private string _issuer { get; set; } = string.Empty;
	private string _reCaptchaSecret { get; set; } = string.Empty;
	private string _resetPasswordCallbackUrl = "/reset-password";
	private string _signInCallbackUrl = "/sign-in";


	/// <summary>
	/// Уникальный идентификатор клиентского приложения.
	/// </summary>
	public string ApplicationCode
	{
		get => _applicationCode;
		set => _applicationCode = !string.IsNullOrWhiteSpace(value)
			? value
			: throw new DataException("Уникальный идентификатор клиентского приложения не должен быть пустым.");
	}

	/// <summary>
	/// Имя клиентского приложения.
	/// </summary>
	public string ApplicationName
	{
		get => _applicationName;
		set => _applicationName = !string.IsNullOrWhiteSpace(value)
			? value
			: throw new DataException("Имя клиентского приложения не должен быть пустым.");
	}

	/// <summary>
	/// <para>Относительный путь обратной ссылки клиентского приложения на страницу регистрации.</para>
	/// <para>По умолчанию "/email-confirmation".</para>
	/// </summary>
	public string ConfirmEmailCallbackUrl
	{
		get => _confirmEmailCallbackUrl;
		set => _confirmEmailCallbackUrl = !string.IsNullOrWhiteSpace(value)
			? value
			: throw new DataException("Относительный путь обратной ссылки клиентского приложения на страницу подтверждения адреса электронной почты не должен быть пустым.");
	}

	/// <summary>
	/// <para>Относительный путь обратной ссылки клиентского приложения на страницу завершения регистрации.</para>
	/// <para>По умолчанию "/sign-up".</para>
	/// </summary>
	public string CompleteRegistrationCallbackUrl
	{
		get => _completeRegistrationCallbackUrl;
		set => _completeRegistrationCallbackUrl = !string.IsNullOrWhiteSpace(value)
			? value
			: throw new DataException("Относительный путь обратной ссылки клиентского приложения на страницу завершения регистрации не должен быть пустым.");
	}

	/// <summary>
	/// Адресная строка клиентского приложения.
	/// </summary>
	public string Issuer
	{
		get => _issuer;
		set => _issuer = !string.IsNullOrWhiteSpace(value)
			? value
			: throw new DataException("Адресная строка клиентского приложения не должнав быть пустой.");
	}

	/// <summary>
	/// Секретный ключ reCaptcha, используемый на стороне сервера для проверки проходждения.
	/// </summary>
	public string ReCaptchaSecret
	{
		get => _reCaptchaSecret;
		set => _reCaptchaSecret = !string.IsNullOrWhiteSpace(value)
			? value
			: throw new DataException("Секретный ключ reCaptcha не должен быть пустым.");
	}

	/// <summary>
	/// <para>Относительный путь обратной ссылки клиентского приложения на страницу изменения пароля.</para>
	/// <para>По умолчанию "/reset-password".</para>
	/// </summary>
	public string ResetPasswordCallbackUrl
	{
		get => _resetPasswordCallbackUrl;
		set => _resetPasswordCallbackUrl = !string.IsNullOrWhiteSpace(value)
			? value
			: throw new DataException("Относительный путь обратной ссылки клиентского приложения на страницу изменения пароля не должен быть пустым.");
	}

	/// <summary>
	/// <para>Относительный путь обратной ссылки клиентского приложения на страницу входа в личный кабинет.</para>
	/// <para>По умолчанию "/sign-in".</para>
	/// </summary>
	public string SignInCallbackUrl
	{
		get => _signInCallbackUrl;
		set => _signInCallbackUrl = !string.IsNullOrWhiteSpace(value)
			? value
			: throw new DataException("Относительный путь обратной ссылки клиентского приложения на страницу входа в личный кабинет.");
	}
}

using Universe.Core.Membership.Dto;

namespace Universe.Core.Membership.Services;

public interface IAccountService
{
	/// <summary>
	/// Выполняет подтверждение адреса электронной почты указанного пользователя.
	/// </summary>
	/// <param name="emailConfirmation">Данные, содержащие идентификатор пользователя, и проверочный токен.</param>
	Task ConfirmEmail(EmailConfirmation emailConfirmation, CancellationToken ct = default);

	/// <summary>
	/// Возвращает личные данные профиля текущего пользователя.
	/// </summary>
	/// <returns>Экземпляр класса <see cref="Universe.Core.Membership.UserProfilePersonal"></see>, 
	/// иначе null.</returns>
	Task<UserProfilePersonal?> GetProfilePersonal(CancellationToken ct = default);

	/// <summary>
	/// Возвращает данные сессии текущего пользователя. В противно случае результатом будет null.
	/// </summary>
	/// <param name="userId">Идентификатор пользователя.</param>
	/// <returns>Если пользователь авторизован, то экземпляр класса <see cref="Universe.Core.Membership.SignInResponseSessionData"></see>, 
	/// иначе null.</returns>
	Task<SignInResponseSessionData?> GetSessionData(Guid userId, CancellationToken ct = default);

	/// <summary>
	/// Определяет, считается ли в данном сервисе логин пользователя и адрес электронной почты одним и тем же параметром.
	/// </summary>
	bool IsSameUserNameEmail();

	/// <summary>
	/// Инициирует процедуру восстановления пароля для указанного пользователя.
	/// </summary>
	/// <param name="passwordRecovery">Данные пользователя.</param>
	Task RecoverPassword(PasswordRecovery passwordRecovery, CancellationToken ct = default);

	/// <summary>
	/// Производит замену пароля.
	/// </summary>
	/// <param name="passwordReset">Данные пользователя и новый пароль.</param>
	Task ResetPassword(PasswordReset passwordReset, CancellationToken ct = default);

	/// <summary>
	/// Определяет, доступна ли самостоятельная регистрация.
	/// </summary>
	bool SelfSignUpAllowed();

	/// <summary>
	/// Выполняет вход в систему.
	/// </summary>
	/// <param name="signInRequest">Данные для входа.</param>
	/// <returns>Результат выполнения операции. В случае успеха содержит данные вошедшего в систему пользователя.</returns>
	Task<SignInResponse> SignIn(SignInRequest signInRequest, CancellationToken ct = default);

	/// <summary>
	/// Выполняет выход из системы текущего пользователя.
	/// </summary>
	Task SignOut();

	/// <summary>
	/// Выполняет регистрацию нового пользователя в системе.
	/// </summary>
	/// <param name="signUpRequest">Данные для регистрации.</param>
	/// <returns>Результат выполнения операции.</returns>
	Task<SignUpResponse> SignUp(SignUpRequest signUpRequest, CancellationToken ct = default);
}

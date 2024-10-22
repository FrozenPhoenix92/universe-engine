using Universe.Core.AppConfiguration.Exceptions;
using Universe.Core.AppConfiguration.Services;
using Universe.Core.Data;
using Universe.Core.Exceptions;
using Universe.Core.Extensions;
using Universe.Core.Membership.Configuration;
using Universe.Core.Membership.Dto;
using Universe.Core.Membership.Exceptions;
using Universe.Core.Membership.Model;
using Universe.Core.Utils;
using Universe.Messaging;
using Universe.Messaging.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Serilog;

using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Universe.Core.Membership.Services;

public class AccountService : IAccountService
{
	private readonly IAppSettingsService _appSettingsService;
	private readonly IDbContext _context;
	private readonly IEmailMessageService _emailMessageService;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IdentitySettings _identitySettings;
	private readonly RoleManager<Role> _roleManager;
	private readonly SignInManager<User> _signInManager;
	private readonly UserManager<User> _userManager;


	public AccountService(
		IHttpContextAccessor httpContextAccessor,
		UserManager<User> userManager,
		SignInManager<User> signInManager,
		RoleManager<Role> roleManager,
		IDbContext context,
		IAppSettingsService appSettingsService,
		IEmailMessageService emailMessageService,
		IOptionsSnapshot<IdentitySettings> identitySettingsOptions)
	{
		VariablesChecker.CheckIsNotNull(httpContextAccessor, nameof(httpContextAccessor));
		VariablesChecker.CheckIsNotNull(httpContextAccessor.HttpContext, nameof(httpContextAccessor.HttpContext));
		VariablesChecker.CheckIsNotNull(userManager, nameof(userManager));
		VariablesChecker.CheckIsNotNull(signInManager, nameof(signInManager));
		VariablesChecker.CheckIsNotNull(roleManager, nameof(roleManager));
		VariablesChecker.CheckIsNotNull(context, nameof(context));
		VariablesChecker.CheckIsNotNull(appSettingsService, nameof(appSettingsService));
		VariablesChecker.CheckIsNotNull(emailMessageService, nameof(emailMessageService));
		VariablesChecker.CheckIsNotNull(identitySettingsOptions, nameof(identitySettingsOptions));
		VariablesChecker.CheckIsNotNull(identitySettingsOptions.Value, nameof(identitySettingsOptions.Value));

		_httpContextAccessor = httpContextAccessor;
		_userManager = userManager;
		_signInManager = signInManager;
		_roleManager = roleManager;
		_context = context;
		_appSettingsService = appSettingsService;
		_emailMessageService = emailMessageService;
		_identitySettings = identitySettingsOptions.Value;
	}


	private string DomainUrl => $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host.Value}";


	public async Task ConfirmEmail(EmailConfirmation emailConfirmation, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(emailConfirmation, nameof(emailConfirmation));

		var user = await _userManager.FindByIdAsync(emailConfirmation.UserId.ToString());

		if (user is null)
			throw new ObjectNotExistsException("Пользователь с указанным идентификатором не существует.");

		var result = await _userManager.ConfirmEmailAsync(user, DecodeToken(emailConfirmation.Token));

		if (!result.Succeeded)
			throw new BusinessException(string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
	}

	public async Task<UserProfilePersonal?> GetProfilePersonal(CancellationToken ct = default)
	{
		if (_httpContextAccessor.HttpContext?.User.Identity is null || !_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
			return null;

		Guid.TryParse(_httpContextAccessor.HttpContext.GetUserId(), out var userId);

		if (userId == default)
			throw new ConflictException("Одновременно с тем, что запрос определяется, как от авторизованного пользователя, " +
				"в нём не содержится идентификатор этого пользователя.");

		var user = await _userManager.Users
			.Include(x => x.UserRoles).ThenInclude(x => x.Role).ThenInclude(x => x.RolePermissions).ThenInclude(x => x.Permission)
			.Include(x => x.UserPermissions).ThenInclude(x => x.Permission)
			.AsNoTracking()
			.SingleOrDefaultAsync(x => x.Id == userId, ct);

		if (user is null)
			throw new ConflictException("Идентифицированный, как авторизованный, пользователь, " +
				"выполняющий данный запрос, не существует в базе данных.");

		var userProfilePersonal = new UserProfilePersonal();

		SetProfilePersonal(user, userProfilePersonal);

		return userProfilePersonal;
	}

	public async Task<SignInResponseSessionData?> GetSessionData(Guid userId, CancellationToken ct = default)
	{
		var user = await _userManager.Users
			.Include(x => x.UserRoles).ThenInclude(x => x.Role).ThenInclude(x => x.RolePermissions).ThenInclude(x => x.Permission)
			.Include(x => x.UserPermissions).ThenInclude(x => x.Permission)
			.AsNoTracking()
			.SingleOrDefaultAsync(x => x.Id == userId, ct);

		if (user is null)
			throw new ConflictException("Идентифицированный, как авторизованный, пользователь, " +
				"выполняющий данный запрос, не существует в базе данных.");

		var sessionData = new SignInResponseSessionData();

		SetSessionData(user, sessionData);

		return sessionData;
	}

	public bool IsSameUserNameEmail() => _identitySettings.SameUserNameEmail;

	public async Task<bool> IsSuperAdminExist(CancellationToken ct = default)
		=> await _context.Set<UserRole>()
		.Include(x => x.Role)
		.AnyAsync(x => x.Role.Name == CoreRoles.SuperAdminRoleName, ct);

	public async Task RecoverPassword(PasswordRecovery passwordRecovery, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(passwordRecovery, nameof(passwordRecovery));

		var signUpSettings = GetRequiredSettings<SignUpSettings>();

		var user = await _userManager.FindByEmailAsync(passwordRecovery.Email);

		if (user is null)
			throw new ObjectNotExistsException("Пользователь с указанным адресом электронной почты не существует.");

		if (await _userManager.IsLockedOutAsync(user))
			throw new ForbiddenException("Заблокированый пользователь не может совершать данную операцию.");

		if (!user.EmailConfirmed)
			throw new ForbiddenException("Пользователь с неподтверждённым адресом электронной почты не может совершать данную операцию.");

		var token = EncodeToken(await _userManager.GeneratePasswordResetTokenAsync(user));

		var extraVars = GetEmailGlobalValiablesFromUser(user);
		extraVars[TemplateVariableNames.CallbackUrl] = $"{DomainUrl}{signUpSettings.ResetPasswordCallbackUrl}?userId={user.Id}&token={token}";

		await SendEmail(TemplateCodes.EmailConfirmed, extraVars, user.Email, ct);
	}

	public async Task ResetPassword(PasswordReset passwordReset, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(passwordReset, nameof(passwordReset));

		var user = await _userManager.FindByIdAsync(passwordReset.UserId.ToString());

		if (user is null)
			throw new ObjectNotExistsException("Пользователь с указанным адресом электронной почты не существует.");

		if (await _userManager.IsLockedOutAsync(user))
			throw new ForbiddenException("Заблокированый пользователь не может совершать данную операцию.");

		var result = await _userManager.ResetPasswordAsync(user, DecodeToken(passwordReset.Token), passwordReset.Password);
		if (!result.Succeeded)
		{
			throw new BusinessException(string.Join(Environment.NewLine, result.Errors.Select(x => x.Description).ToList()));
		}

		try
		{
			await SendEmail(TemplateCodes.PasswordChanged, GetEmailGlobalValiablesFromUser(user), user.Email, ct);
		}
		catch (Exception ex)
		{
			Log.Error($"Ошибка отправки электронного письма в процессе регистрации: {ex.Message}");
		}
	}

	public bool SelfSignUpAllowed()
	{
		var signUpSettings = GetRequiredSettings<SignUpSettings>();
		return signUpSettings.SelfSignUpEnabled;
	}

	public async Task<SignInResponse> SignIn(SignInRequest signInRequest, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(signInRequest, nameof(signInRequest));

		var authorizationSettings = GetRequiredSettings<AuthorizationSettings>();

		var user = await _userManager.Users
		   .Include(o => o.UserRoles).ThenInclude(o => o.Role)
		   .FirstOrDefaultAsync(u => u.Email == signInRequest.Login || u.UserName == signInRequest.Login, ct);

		if (user is null)
		{
			throw new WrongCredentialsException(
				"Неверный логин или пароль.",
				$"Пользователь с логином или адресом электронной почты или '{signInRequest.Login}' не существует.");
		}

		var logInResult = await _signInManager.PasswordSignInAsync(
			user,
			signInRequest.Password,
			false,
			authorizationSettings.LimitFailedAccessAttempts);

		if (!logInResult.Succeeded)
		{
			throw new WrongCredentialsException(
				"Неверный логин или пароль.",
				$"Пользователь с логином '{signInRequest.Login}' ввёл неверный пароль.");
		}

		var signInResponse = new SignInResponse
		{
			SessionData = new SignInResponseSessionData
            {
				UserId = user.Id
			}
		};

		if (logInResult.RequiresTwoFactor)
        {
			signInResponse.TwoFactorCodeRequired = true;
        }
        else
		{
			SetSessionData(user, signInResponse.SessionData);
		}

		return signInResponse;
	}

	public async Task SignOut()
	{
		if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated is false) return;

		await _signInManager.SignOutAsync();
	}

	public async Task<SignUpResponse> SignUp(SignUpRequest signUpRequest, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(signUpRequest, nameof(signUpRequest));

		var signUpSettings = GetRequiredSettings<SignUpSettings>();

		if (!signUpSettings.SelfSignUpEnabled)
			throw new ForbiddenException("Самостоятельная регистрация запрещена.");

		if (string.IsNullOrWhiteSpace(signUpRequest.UserName))
			throw new ValidationException("Логин не может быть пустым.");

		if (await _context.Set<User>().AnyAsync(x => x.UserName == signUpRequest.UserName, ct))
			throw new BusinessException("Пользователь с указанным логином уже зарегистрирован.");

		var user = new User(signUpRequest.UserName)
		{
			FirstName = signUpRequest.FirstName?.Trim(),
			LastName = signUpRequest.LastName?.Trim(),
			Email = _identitySettings.SameUserNameEmail ? signUpRequest.UserName?.Trim() : signUpRequest.Email?.Trim(),
		};

		if (string.IsNullOrWhiteSpace(user.Email))
			throw new ValidationException("Адрес электронной почты не может быть пустым.");

		if (await _context.Set<User>().AnyAsync(x => x.Email == user.Email, ct))
			throw new BusinessException("Пользователь с указанным адресом электронной почты уже зарегистрирован.");

		/*var registrationResult = new RegistrationResponse
		{
			PwnedResult = await CheckPwnedPassword(dto)
		};*/

		if (!string.IsNullOrEmpty(signUpSettings.SelfSignUpRole))
		{
			var selfSignUpRole = await _roleManager.FindByNameAsync(signUpSettings.SelfSignUpRole);

			if (selfSignUpRole is null)
			{
				Log.Warning("Роль с названием, указанным в настройках, присваиваемая по умолчанию пользователям после " +
					"успешного прохождения самостоятельной регистрации, не существует в БД.");
			}
			else
			{
				user.UserRoles.Add(new UserRole { RoleId = selfSignUpRole.Id });
			}
		}

		var result = await _userManager.CreateAsync(user, signUpRequest.Password);
		if (!result.Succeeded)
			throw new BusinessException(string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));

		var signUpResponse = new SignUpResponse();

		// await _userManager.UpdateAsync(user);

		if (signUpSettings.RequireApproval)
		{
			signUpResponse.ApprovalRequired = true;
		}
		else
		{
			if (signUpSettings.RequireConfirmedEmail)
			{
				signUpResponse.EmailConfirmationRequired = true;

				var token = EncodeToken(await _userManager.GenerateEmailConfirmationTokenAsync(user));

				try
				{
					var extraVars = GetEmailGlobalValiablesFromUser(user);
					extraVars[TemplateVariableNames.CallbackUrl] = $"{DomainUrl}{signUpSettings.ConfirmEmailCallbackUrl}?userId={user.Id}&token={token}";

					await SendEmail(TemplateCodes.EmailConfirmed, extraVars, user.Email, ct);
				}
				catch (Exception ex)
				{
					Log.Error($"Ошибка отправки электронного письма в процессе регистрации: {ex.Message}");
				}
			}

			// Добавить обработку подтверждения номера телефона
		}

		if (signUpSettings.AutoSignIn)
        {
			if (signUpResponse.ApprovalRequired)
			{
				Log.Warning("Автоматический вход в систему после регистрации невозможен, если требуется подтверждение аккаунта.");
			}
			else if (signUpResponse.EmailConfirmationRequired)
			{
				Log.Warning("Автоматический вход в систему после регистрации невозможен, если требуется подтверждение адреса электронной почты.");
			}
			else if (signUpResponse.PhoneNumberConfirmationRequired)
			{
				Log.Warning("Автоматический вход в систему после регистрации невозможен, если требуется подтверждение номера телефона.");
			}
			else 
			{
				var logInResult = await _signInManager.PasswordSignInAsync(user, signUpRequest.Password, false, false);

				if (!logInResult.Succeeded)
					throw new ConflictException("Ошибка автоматического входа в систему после прохождения регистрации.");

				signUpResponse.SignInResponse = new SignInResponse 
				{
					SessionData = new SignInResponseSessionData
					{
						UserId = user.Id
					}
				};

				if (logInResult.RequiresTwoFactor)
				{
					signUpResponse.SignInResponse.TwoFactorCodeRequired = true;
				}
				else
				{
					SetSessionData(user, signUpResponse.SignInResponse.SessionData);
				}
			}
		}

		return signUpResponse;
	}


	private static string DecodeToken(string? token) => Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token ?? string.Empty));

	private static string EncodeToken(string token) => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

	private IDictionary<string, string> GetEmailGlobalValiablesFromUser(User user)
		=> new Dictionary<string, string>()
		{
			[GlobalTemplateVariableNames.UserName] = user.FirstName ?? string.Empty,
			[GlobalTemplateVariableNames.UserSurname] = user.LastName ?? string.Empty,
			[GlobalTemplateVariableNames.UserEmail] = user.Email ?? string.Empty
		};


	private T GetRequiredSettings<T>() where T: class => _appSettingsService.GetSettings<T>()
		?? throw new MissedConfigurationSectionException(typeof(T).Name);

	private void SetProfilePersonal(User user, UserProfilePersonal userProfilePersonal)
	{
		userProfilePersonal.Email = user.Email;
		userProfilePersonal.PhoneNumber = user.PhoneNumber;
		userProfilePersonal.FirstName = user.FirstName;
		userProfilePersonal.LastName = user.LastName;
		userProfilePersonal.Roles = user.UserRoles.Select(x => x.Role?.Name ?? throw new ConflictException("Имя роли не задано."));
		userProfilePersonal.Permissions = user.UserPermissions
			.Select(x => x.Permission?.Name ?? string.Empty)
			.Union(user.UserRoles.SelectMany(x =>
				x.Role?.RolePermissions.Select(y => y.Permission?.Name ?? string.Empty)
				?? Array.Empty<string>()))
			.Distinct();
	}

	private void SetSessionData(User user, SignInResponseSessionData sessionData)
	{
		sessionData.UserId = user.Id;
		sessionData.UserName = user.UserName;
		sessionData.Roles = user.UserRoles.Select(x => x.Role?.Name ?? throw new ConflictException("Имя роли не задано."));
		sessionData.Permissions = user.UserPermissions
			.Select(x => x.Permission?.Name ?? string.Empty)
			.Union(user.UserRoles.SelectMany(x =>
				x.Role?.RolePermissions.Select(y => y.Permission?.Name ?? string.Empty)
				?? Array.Empty<string>()))
			.Distinct();
	}

	private async Task SendEmail(string emailTemplateCode, IDictionary<string, string> extraVariables, string reciever, CancellationToken ct)
	{
		if (_emailMessageService is null)
			throw new ConflictException("Ошибка в процессе регистрации: невозможно отправить электронное письмо, так как сервис управления электронными письмами не задан.");

		var mail = await _emailMessageService.Build(
			emailTemplateCode,
			extraVariables,
			ct);
		mail.To.Add(reciever);

		await _emailMessageService.Send(mail, ct);
	}
}

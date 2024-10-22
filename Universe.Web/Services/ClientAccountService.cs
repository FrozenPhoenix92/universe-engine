using Universe.Core.Data;
using Universe.Core.Exceptions;
using Universe.Core.Membership.Exceptions;
using Universe.Core.Utils;
using Universe.Messaging.Services;
using Universe.Web.Configuration;
using Universe.Web.Dto.Client;
using Universe.Web.Model;
using Universe.Web.Utils;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Serilog;

using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;

using System.Text;
using Universe.Core.AppConfiguration.Services;

namespace Universe.Web.Services;

public class ClientAccountService : IClientAccountService
{
	private readonly IAppSettingsService _appSettingsService;
	private readonly ClientAppJwtSettings _clientAppJwtSettings;
	private readonly List<ClientApplicationSettings> _clientApplicationSettings;
	private readonly IClientOperationConfirmationTokenService _clientOperationConfirmationTokenService;
	private readonly IDbContext _dataContext;
	private readonly IEmailMessageService _emailMessageService;
	private readonly IReCaptchaService _reCaptchaService;


	public ClientAccountService(
		IOptions<ClientAppJwtSettings> clientAppJwtSettingsOptions,
		IOptions<List<ClientApplicationSettings>> clientApplicationSettingsOptions,
		IDbContext dataContext,
		IEmailMessageService emailMessageService,
		IClientOperationConfirmationTokenService clientOperationConfirmationTokenService,
		IAppSettingsService appSettingsService,
		IReCaptchaService reCaptchaService)
	{
		VariablesChecker.CheckIsNotNull(clientAppJwtSettingsOptions, nameof(clientAppJwtSettingsOptions));
		VariablesChecker.CheckIsNotNull(clientAppJwtSettingsOptions.Value, nameof(clientAppJwtSettingsOptions.Value));
		VariablesChecker.CheckIsNotNull(clientApplicationSettingsOptions, nameof(clientApplicationSettingsOptions));
		VariablesChecker.CheckIsNotNull(clientApplicationSettingsOptions.Value, nameof(clientApplicationSettingsOptions.Value));
		VariablesChecker.CheckIsNotNull(dataContext, nameof(dataContext));
		VariablesChecker.CheckIsNotNull(emailMessageService, nameof(emailMessageService));
		VariablesChecker.CheckIsNotNull(appSettingsService, nameof(appSettingsService));
		VariablesChecker.CheckIsNotNull(reCaptchaService, nameof(reCaptchaService));

		_clientAppJwtSettings = clientAppJwtSettingsOptions.Value;
		_clientApplicationSettings = clientApplicationSettingsOptions.Value;
		_dataContext = dataContext;
		_emailMessageService = emailMessageService;
		_clientOperationConfirmationTokenService = clientOperationConfirmationTokenService;
		_appSettingsService = appSettingsService;
		_reCaptchaService = reCaptchaService;
	}


	public async Task ChangePassword(ClientChangePasswordRequest request, CancellationToken ct = default)
	{
		var client = await _dataContext.Set<Client>().SingleOrDefaultAsync(x => x.Id == request.ClientId, ct);

		if (client == null)
		{
			throw new ObjectNotExistsException("Пользователь с указанным идентификатором не существует.");
		}

		var requestPasswordHash = PasswordHelper.HashUsingPbkdf2(request.OldPassword, Convert.FromBase64String(client.PasswordSalt!));

		if (client.PasswordHash != requestPasswordHash)
		{
			throw new BusinessException("Неверно указан старый пароль");
		}

		ValidatePassword(request.NewPassword);
		SetClientPassword(client, request.NewPassword);

		await _dataContext.SaveChangesAsync(ct);
	}
	
	public async Task<ClientSignInResult> ConfirmEmail(ClientConfirmEmailRequest request, CancellationToken ct = default)
	{
		var client = await _dataContext.Set<Client>().SingleOrDefaultAsync(x => x.Id == request.UserId, ct);

		if (client == null)
		{
			throw new ObjectNotExistsException("Пользователь с указанным идентификатором не существует.");
		}

		if (client.AccountConfirmed)
		{
			throw new BusinessException("Аккаунт уже подтверждён.");
		}

		if (!await _clientOperationConfirmationTokenService.ValidateTokenValue(
				DecodeToken(request.Token) ?? string.Empty,
				client.Id,
				ClientOperationConfirmationTokenService.EmailConfirmationOperationName,
				ct))
		{
			throw new BusinessException("Операция не прошла проверку валидности.");
		}

		client.AccountConfirmed = true;

		if (string.IsNullOrEmpty(client.PasswordHash))
		{
			ValidatePassword(request.Password);
			SetClientPassword(client, request.Password);
		}

		await _dataContext.SaveChangesAsync(ct);

		await _clientOperationConfirmationTokenService.DeleteToken(
			client.Id,
			ClientOperationConfirmationTokenService.EmailConfirmationOperationName,
			ct);

		return new ClientSignInResult
		{
			AccessToken = GetClientAuthToken(client),
			SessionData = await GetClientSessionData(client.Id, ct)
		};
	}

	public async Task ForgotPassword(ClientForgotPasswordRequest request, CancellationToken ct = default)
	{
		var client = await _dataContext.Set<Client>().SingleOrDefaultAsync(x => EF.Functions.Like(x.Email, request.Email), ct);

		if (client == null)
		{
			throw new ObjectNotExistsException("Пользователь с указанным адресом электронной почты не существует.");
		}

		var tokenEntity = await _clientOperationConfirmationTokenService.GetToken(
			client.Id,
			ClientOperationConfirmationTokenService.PasswordRecoveryOperationName,
			ct)
			?? await _clientOperationConfirmationTokenService.GenerateToken(
				client.Id,
				ClientOperationConfirmationTokenService.PasswordRecoveryOperationName,
				0,
				ct);

		if (tokenEntity.UpdatedAt.AddMinutes(2) >= DateTime.UtcNow)
		{
			throw new BusinessException("Повтор операции сброса пароля возможен не ранее, чем через две минуты.");
		}

		var token = EncodeToken(tokenEntity.Token);

		try
		{
			var clientApplicationSettingsItem = _clientApplicationSettings.SingleOrDefault(x => x.ApplicationCode == client.AppCode);

			if (clientApplicationSettingsItem == null)
			{
				throw new BusinessException("Несуществующий код приложения.");
			}

			var extraVars = new Dictionary<string, string>
			{
				[TemplateVariableNames.CallbackUrl] =
					$"{clientApplicationSettingsItem.Issuer}{clientApplicationSettingsItem.ResetPasswordCallbackUrl}?userId={client.Id}&token={token}"
			};

			await SendEmail(TemplateCodes.ClientResetPassword, extraVars, client.Email, ct);
		}
		catch (Exception ex)
		{
			Log.Error($"Ошибка отправки электронного письма в процессе регистрации: {ex.Message}");
		}
	}

	public async Task<string> GetClientAuthToken(Guid clientId, CancellationToken ct = default)
	{
		var client = await _dataContext.Set<Client>().SingleOrDefaultAsync(x => x.Id == clientId, ct);

		return client is null
			? throw new ObjectNotExistsException("Пользователь с указанным идентификатором не существует.")
			: GetClientAuthToken(client);
	}

	public string GetClientAuthToken(Client client)
	{
		var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_clientAppJwtSettings.SigningKey));
		var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, client.Id.ToString()),
			new Claim(ClaimTypes.Email, client.Email ?? string.Empty),
			new Claim(ClaimTypes.NameIdentifier, client.Id.ToString())
		};

		if (client.AccountConfirmed)
		{
			claims.Add(new Claim(ProjectClaims.ClientConfirmedAccountClaimName, "true"));
		}

		var clientApplicationSettingsItem = _clientApplicationSettings.SingleOrDefault(x => x.ApplicationCode == client.AppCode);

		if (clientApplicationSettingsItem == null)
		{
			throw new BusinessException("Несуществующий код приложения.");
		}

		var tokenOptions = new JwtSecurityToken(
			issuer: clientApplicationSettingsItem.Issuer,
			audience: _clientAppJwtSettings.Audience,
			claims: claims,
			expires: DateTime.Now.AddMinutes(_clientAppJwtSettings.ExpireTime),
			signingCredentials: signinCredentials
		);

		return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
	}

	public async Task<ClientSessionData> GetClientSessionData(Guid clientId, CancellationToken ct = default)
	{
		var client = await _dataContext.Set<Client>().SingleOrDefaultAsync(x => x.Id == clientId, ct);

		return client is null
			? throw new ObjectNotExistsException("Пользователь с указанным идентификатором не существует.")
			: new ClientSessionData
			{
				AccountConfirmed = client.AccountConfirmed,
				Blocked = client.Blocked,
				Email = client.Email,
				Id = clientId
			};
	}

	public async Task<ClientSignInResult> ResetPassword(ClientResetPasswordRequest request, CancellationToken ct = default)
	{
		var client = await _dataContext.Set<Client>().SingleOrDefaultAsync(x => x.Id == request.UserId, ct);

		if (client == null)
		{
			throw new ObjectNotExistsException("Пользователь с указанным идентификатором не существует.");
		}

		ValidatePassword(request.Password);

		if (!await _clientOperationConfirmationTokenService.ValidateTokenValue(
				DecodeToken(request.Token) ?? string.Empty,
				client.Id,
				ClientOperationConfirmationTokenService.PasswordRecoveryOperationName,
				ct))
		{
			throw new BusinessException("Операция не прошла проверку валидности.");
		}

		client.AccountConfirmed = true;
		SetClientPassword(client, request.Password);

		await _dataContext.SaveChangesAsync(ct);

		await _clientOperationConfirmationTokenService.DeleteToken(
			client.Id,
			ClientOperationConfirmationTokenService.EmailConfirmationOperationName,
			ct);

		return new ClientSignInResult
		{
			AccessToken = GetClientAuthToken(client),
			SessionData = await GetClientSessionData(client.Id, ct)
		};
	}

	public string SetAutoGeneratedPasswordForClient(Client client)
	{
		var password = GeneratePassword();

		SetClientPassword(client, password);

		return password;
	}

	public async Task<ClientSignInResult> SignIn(ClientSignInRequest request, CancellationToken ct = default)
	{
		var client = await _dataContext.Set<Client>().SingleOrDefaultAsync(x => EF.Functions.Like(x.Email, $"{request.Email}%"), ct);

		if (client == null)
		{
			throw new LoginFailedException("Неверные данные.", $"Пользователь с адресом электронной почты '{request.Email}' не существует.");
		}

		var requestPasswordHash = PasswordHelper.HashUsingPbkdf2(request.Password, Convert.FromBase64String(client.PasswordSalt!));

		if (client.PasswordHash != requestPasswordHash)
		{
			throw new LoginFailedException("Неверные данные.", $"Пользователь с адресом электронной почты '{request.Email}' ввёл неверный пароль.");
		}

		if (!await _reCaptchaService.ValidateResponse(request.CaptchaResponse, client.AppCode, ct))
		{
			throw new LoginFailedException("Проверка против робота не пройдена.", $"Пользователь с адресом электронной почты '{request.Email}' не прошёл проверку reCaptcha V2.");
		}

		if (!client.AccountConfirmed)
		{
			return new ClientSignInResult
			{
				EmailConfirmationRequired = true,
			};
		}

		return new ClientSignInResult
		{
			AccessToken = GetClientAuthToken(client),
			SessionData = await GetClientSessionData(client.Id, ct)
		};
	}

	public async Task<ClientSignInResult> SignUp(ClientSignUpRequest request, CancellationToken ct = default)
	{
		ValidatePassword(request.Password);

		Client? client;

		if (request.UserId != null)
		{
			client = await _dataContext.Set<Client>().SingleOrDefaultAsync(x => x.Id == request.UserId, ct);

			if (client == null)
			{
				throw new ObjectNotExistsException("Пользователь с указанным идентификатором не существует.");
			}

			if (client.AccountConfirmed)
			{
				throw new ObjectNotExistsException("Аккаунт уже подтверждён.");
			}
			
			if (!await _clientOperationConfirmationTokenService.ValidateTokenValue(
				DecodeToken(request.Token) ?? string.Empty,
				client.Id,
				ClientOperationConfirmationTokenService.EmailConfirmationOperationName,
				ct))
			{
				throw new BusinessException("Операция не прошла проверку валидности.");
			}

			client.AccountConfirmed = true;
			client.SignUpDate = DateTime.UtcNow;
			SetClientPassword(client, request.Password);

			await _dataContext.SaveChangesAsync(ct);

			await _clientOperationConfirmationTokenService.DeleteToken(
				client.Id,
				ClientOperationConfirmationTokenService.EmailConfirmationOperationName,
				ct);

			return new ClientSignInResult
			{
				AccessToken = GetClientAuthToken(client),
				SessionData = await GetClientSessionData(client.Id, ct)
			};
		}

		if (!RegexUtilities.IsValidEmail(request.Email ?? string.Empty))
		{
			throw new ValidationException("Указанный адрес электронной почты имеет недопустимый формат.");
		}

		if (string.IsNullOrWhiteSpace(request.AppCode))
		{
			throw new BusinessException("Неверный код приложения.");
		}
		else
		{
			client = await _dataContext.Set<Client>()
				.SingleOrDefaultAsync(x => EF.Functions.Like(x.Email, request.Email) && EF.Functions.Like(x.AppCode, request.AppCode), ct)
				?? new Client
				{
					Email = request.Email!,
					SignUpDate = DateTime.UtcNow,
					AppCode = request.AppCode
				};
		}

		if (string.IsNullOrEmpty(client.PasswordHash))
		{
			SetClientPassword(client, request.Password);
		}
		else
		{
			throw new BusinessException("Пользователь с указанным адресом электронной почты уже существует.");
		}

		if (client.Id == default)
		{
			if (await _dataContext.Set<Client>().AnyAsync(x => EF.Functions.Like(x.Email, request.Email) && EF.Functions.Like(x.AppCode, request.AppCode), ct))
			{
				throw new BusinessException("Пользователь с указанным адресом электронной почты уже существует.");
			}

			await _dataContext.Set<Client>().AddAsync(client, ct);
		}
		
		await _dataContext.SaveChangesAsync(ct);

		var tokenEntity = await _clientOperationConfirmationTokenService.GetToken(
			client.Id,
			ClientOperationConfirmationTokenService.EmailConfirmationOperationName,
			ct)
			?? await _clientOperationConfirmationTokenService.GenerateToken(
				client.Id,
				ClientOperationConfirmationTokenService.EmailConfirmationOperationName,
				0,
				ct);

		if (tokenEntity.UpdatedAt.AddMinutes(2) >= DateTime.UtcNow)
		{
			throw new BusinessException("Повторная отправка ссылки для подтверждения адреса электронной почты возможна не ранее, чем через две минуты.");
		}

		var token = EncodeToken(tokenEntity.Token);

		try
		{
			var clientApplicationSettingsItem = _clientApplicationSettings.SingleOrDefault(x => x.ApplicationCode == client.AppCode);

			if (clientApplicationSettingsItem == null)
			{
				throw new BusinessException("Несуществующий код приложения.");
			}

			var extraVars = new Dictionary<string, string>
			{
				[TemplateVariableNames.CallbackUrl] = $"{clientApplicationSettingsItem.Issuer}{clientApplicationSettingsItem.ConfirmEmailCallbackUrl}?userId={client.Id}&token={token}"
			};

			await SendEmail(TemplateCodes.ClientEmailConfirmed, extraVars, client.Email, ct);
		}
		catch (Exception ex)
		{
			Log.Error($"Ошибка отправки электронного письма в процессе регистрации: {ex.Message}");
		}

		return new ClientSignInResult
		{
			EmailConfirmationRequired = true,
		};
	}


	private static string DecodeToken(string? token) => Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token ?? string.Empty));

	private static string EncodeToken(string token) => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

	private static string GeneratePassword()
	{
		string[] randomChars = [
			"ABCDEFGHJKLMNOPQRSTUVWXYZ",
            "abcdefghijkmnopqrstuvwxyz",
            "0123456789"
        ];

		Random rand = new Random(Environment.TickCount);
		List<char> chars = new List<char>();

		chars.Insert(rand.Next(0, chars.Count),
			randomChars[0][rand.Next(0, randomChars[0].Length)]);

		chars.Insert(rand.Next(0, chars.Count),
			randomChars[1][rand.Next(0, randomChars[1].Length)]);

		chars.Insert(rand.Next(0, chars.Count),
			randomChars[2][rand.Next(0, randomChars[2].Length)]);

		for (int i = chars.Count; i < 8; i++)
		{
			string rcs = randomChars[rand.Next(0, randomChars.Length)];
			chars.Insert(rand.Next(0, chars.Count),
				rcs[rand.Next(0, rcs.Length)]);
		}

		return new string(chars.ToArray());
	}

	private static void SetClientPassword(Client client, string password)
	{
		var salt = PasswordHelper.GetSecureSalt();

		client.PasswordSalt = Convert.ToBase64String(salt);
		client.PasswordHash = PasswordHelper.HashUsingPbkdf2(password, salt);
	}

	private async Task SendEmail(
		string emailTemplateCode,
		IDictionary<string, string> extraVariables,
		string reciever,
		CancellationToken ct)
	{
		var mail = await _emailMessageService.Build(
			emailTemplateCode,
			extraVariables,
			ct);
		mail.To.Add(reciever);

		await _emailMessageService.Send(mail, ct);
	}

	private void ValidatePassword(string password)
	{
		string[] randomChars = [
			"ABCDEFGHJKLMNOPQRSTUVWXYZ",
			"abcdefghijkmnopqrstuvwxyz",
			"0123456789"
		];

		if (randomChars[0].Intersect(password).Count() == 0)
		{
			throw new ValidationException("Пароль должен содержать минимум одну прописную букву.");
		}

		if (randomChars[1].Intersect(password).Count() == 0)
		{
			throw new ValidationException("Пароль должен содержать минимум одну строчную букву.");
		}

		if (randomChars[2].Intersect(password).Count() == 0)
		{
			throw new ValidationException("Пароль должен содержать минимум одну цифру.");
		}

		if (password.Length < 8)
		{
			throw new ValidationException("Длина пароля должна быть не менее 8 символов.");
		}
	}
}

using AutoMapper;

using Universe.Core.AppConfiguration;
using Universe.Core.Exceptions;
using Universe.Core.Extensions;
using Universe.Core.Infrastructure;
using Universe.Core.Utils;
using Universe.Web.Dto.Client;
using Universe.Web.Filters;
using Universe.Web.Services;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Universe.Web.Controllers.Client;

[EnableCors(ProjectPolicies.ClientAppCorsPolicyName)]
[Route($"{ApiRoutes.ClientApp}account")]
[ValidateClientApplicationCodeHeaderFilter]
[Authorize(AuthenticationSchemes = ProjectAuthenticationSchemes.ClientAppJwtSchemeName)]
public class ClientAccountController : ControllerCore
{
	private readonly IClientAccountService _clientAccountService;
	private readonly IDataService<Model.Client, Guid> _clientDataService;


	public ClientAccountController(
		IMapper mapper,
		IClientAccountService clientAccountService,
		IDataService<Model.Client, Guid> clientDataService) : base(mapper)
	{
		VariablesChecker.CheckIsNotNull(clientAccountService, nameof(clientAccountService));
		VariablesChecker.CheckIsNotNull(clientDataService, nameof(clientDataService));

		_clientAccountService = clientAccountService;
		_clientDataService = clientDataService;
	}

	[HttpPost("change-password")]
	[BlockedClientFilter]
	public async Task<IActionResult> ChangePassword([FromBody] ClientChangePasswordRequest data, CancellationToken ct)
	{
		if (!Guid.TryParse(HttpContext.GetUserId(), out var clientId))
			throw new ConflictException("Одновременно с тем, что запрос определяется, как от авторизованного пользователя, " +
				"в нём не содержится идентификатор этого пользователя.");

		data.ClientId = clientId;

		return await NoContent(async () => await _clientAccountService.ChangePassword(data, ct));
	}

	[HttpPost("confirm-email")]
	[AllowAnonymous]
	public async Task<IActionResult> ConfirmEmail([FromBody] ClientConfirmEmailRequest data, CancellationToken ct)
	{
		if (HttpContext?.User?.Identity?.IsAuthenticated is true)
		{
			throw new ForbiddenException("Вы уже авторизованы.");
		}

		await CheckClientBlock(data.UserId, ct);

		var signInResult = await _clientAccountService.ConfirmEmail(data, ct);

		SetClientAccessToken(signInResult.AccessToken);

		return Ok(signInResult);
	}

	[HttpPost("forgot-password")]
	[AllowAnonymous]
	public async Task<IActionResult> ForgotPassword([FromBody] ClientForgotPasswordRequest data, CancellationToken ct)
	{
		if (HttpContext?.User?.Identity?.IsAuthenticated is true)
		{
			throw new ForbiddenException("Вы уже авторизованы.");
		}

		await CheckClientBlock(data.Email, ct);

		await _clientAccountService.ForgotPassword(data, ct);

		return NoContent();
	}

	[HttpGet("session-data")]
	[AllowAnonymous]
	public async Task<IActionResult> GetSessionData(CancellationToken ct)
	{
		if (HttpContext?.User.Identity is null || !HttpContext.User.Identity.IsAuthenticated)
			return Ok(null);

		Guid.TryParse(HttpContext.GetUserId(), out var clientId);

		if (clientId == default)
			throw new ConflictException("Одновременно с тем, что запрос определяется, как от авторизованного пользователя, " +
				"в нём не содержится идентификатор этого пользователя.");

		return Ok(await _clientAccountService.GetClientSessionData(clientId, ct));
	}

	[HttpGet("refresh-anti-forgery-token")]
	[AllowAnonymous]
	public IActionResult RefreshAntiForgeryToken() => NoContent();

	[HttpPost("reset-password")]
	[AllowAnonymous]
	public async Task<IActionResult> ConfirmEmail([FromBody] ClientResetPasswordRequest data, CancellationToken ct)
	{
		if (HttpContext?.User?.Identity?.IsAuthenticated is true)
		{
			throw new ForbiddenException("Вы уже авторизованы.");
		}

		await CheckClientBlock(data.UserId, ct);

		var signInResult = await _clientAccountService.ResetPassword(data, ct);

		SetClientAccessToken(signInResult.AccessToken);

		return Ok(signInResult);
	}

	[HttpPost("sign-in")]
	[AllowAnonymous]
	public async Task<IActionResult> SignIn([FromBody] ClientSignInRequest data, CancellationToken ct)
	{
		if (HttpContext?.User?.Identity?.IsAuthenticated is true)
		{
			throw new ForbiddenException("Вы уже авторизованы.");
		}

		await CheckClientBlock(data.Email, ct);

		var signInResult = await _clientAccountService.SignIn(data, ct);

		if (!signInResult.EmailConfirmationRequired)
		{
			SetClientAccessToken(signInResult.AccessToken);
		}

		return Ok(signInResult);
	}

	[HttpPost("sign-up")]
	[AllowAnonymous]
	public async Task<IActionResult> SignUp([FromBody] ClientSignUpRequest data, CancellationToken ct)
	{
		if (HttpContext?.User?.Identity?.IsAuthenticated is true)
		{
			throw new ForbiddenException("Вы уже авторизованы.");
		}

		if (HttpContext?.Request.Headers.TryGetValue(ValidateClientApplicationCodeHeaderFilterAttribute.ClientApplicationCodeHeaderName, out var appCode) is true)
		{
			data.AppCode = appCode.ToString();
		}
		else
		{
			throw new ApiException("В запросе не задан код приложения.");
		}

		var signInResult = await _clientAccountService.SignUp(data, ct);

		if (!signInResult.EmailConfirmationRequired)
		{
			SetClientAccessToken(signInResult.AccessToken);
		}
			
		return Ok(signInResult);
	}


	private void SetClientAccessToken(string accessToken)
	{
		if (string.IsNullOrEmpty(Request.Headers.SingleOrDefault(x =>
			x.Key.Equals("AccessToken", StringComparison.InvariantCultureIgnoreCase)).Value))
		{
			Response.Headers.Add("AccessToken", accessToken);
		}
	}

	private async Task CheckClientBlock(Guid clientId, CancellationToken ct)
	{
		var client = await _clientDataService.Get(clientId, ct);

		if (client?.Blocked is true)
		{
			throw new ForbiddenException("Клиент заблокирован.");
		}
	}

	private async Task CheckClientBlock(string email, CancellationToken ct)
	{
		var client = await _clientDataService.Get(x => EF.Functions.Like(email, x.Email), ct);

		if (client?.Blocked is true)
		{
			throw new ForbiddenException("Клиент заблокирован.");
		}
	}
}

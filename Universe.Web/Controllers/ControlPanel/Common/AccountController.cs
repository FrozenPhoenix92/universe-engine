using AutoMapper;

using Universe.Core.AppConfiguration;
using Universe.Core.Exceptions;
using Universe.Core.Extensions;
using Universe.Core.Infrastructure;
using Universe.Core.Membership.Dto;
using Universe.Core.Membership.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System;

namespace Universe.Web.Controllers.ControlPanel;

[Route($"{ApiRoutes.ControlPanel}account")]
public class AccountController : ControllerCore
{
	private readonly IAccountService _accountService;


	public AccountController(IMapper mapper, IAccountService accountService) : base(mapper) => _accountService = accountService;


	[AllowAnonymous]
	[HttpPost("confirm-email")]
	public async Task<IActionResult> ConfirmEmail([FromBody] EmailConfirmation emailConfirmation, CancellationToken ct)
		=> await NoContent(async () => await _accountService.ConfirmEmail(emailConfirmation, ct));

	[HttpGet("profile-personal")]
	public async Task<IActionResult> GetProfilePersonal(CancellationToken ct)
		=> Ok(await _accountService.GetProfilePersonal(ct));

	[HttpGet("session-data")]
	[AllowAnonymous]
	public async Task<IActionResult> GetSessionData(CancellationToken ct)
	{
		if (HttpContext?.User.Identity is null || !HttpContext.User.Identity.IsAuthenticated)
			return Ok(null);

		Guid.TryParse(HttpContext.GetUserId(), out var userId);

		if (userId == default)
			throw new ConflictException("Одновременно с тем, что запрос определяется, как от авторизованного пользователя, " +
				"в нём не содержится идентификатор этого пользователя.");

		return Ok(await _accountService.GetSessionData(userId, ct));
	}

	[HttpPost("sign-out")]
	public async Task<IActionResult> LogOut() => await NoContent(async () => await _accountService.SignOut());

	[HttpGet("is-same-user-name-email")]
	[AllowAnonymous]
	public IActionResult IsSameUserNameEmail() => Ok(_accountService.IsSameUserNameEmail());

	[HttpPost("recover-password")]
	[AllowAnonymous]
	public async Task<IActionResult> RecoverPassword([FromBody] PasswordRecovery passwordRecovery, CancellationToken ct)
		=> await NoContent(async () => await _accountService.RecoverPassword(passwordRecovery, ct));

	[HttpPost("reset-password")]
	[AllowAnonymous]
	public async Task<IActionResult> ResetPassword([FromBody] PasswordReset passwordReset, CancellationToken ct)
		=> await NoContent(async () => await _accountService.ResetPassword(passwordReset, ct));

	[HttpGet("self-sign-up-allowed")]
	[AllowAnonymous]
	public IActionResult SelfSignUpAllowed() => Ok(_accountService.SelfSignUpAllowed());

	[HttpPost("sign-in")]
	[AllowAnonymous]
	public async Task<IActionResult> SignIn([FromBody] SignInRequest signInRequest, CancellationToken ct) 
		=> Ok(await _accountService.SignIn(signInRequest, ct));

	[HttpPost("sign-up")]
	[AllowAnonymous]
	public async Task<IActionResult> SignUp([FromBody] SignUpRequest signUpRequest, CancellationToken ct)
		=> Ok(await _accountService.SignUp(signUpRequest, ct));

	[HttpGet("refresh-anti-forgery-token")]
	[AllowAnonymous]
	public IActionResult RefreshAntiForgeryToken() => NoContent();
}

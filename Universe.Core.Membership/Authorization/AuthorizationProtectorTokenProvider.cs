using Universe.Core.Membership.Configuration;
using Universe.Core.Membership.Model;
using Universe.Core.AppConfiguration.Services;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Universe.Core.Exceptions;

namespace Universe.Core.Membership.Authorization;

public class AuthorizationProtectorTokenProvider : DataProtectorTokenProvider<User>
{
	public const string ChangePhoneNumberTokenPurpose = UserManager<User>.ChangePhoneNumberTokenPurpose;
	public const string ConfirmEmailTokenPurpose = UserManager<User>.ConfirmEmailTokenPurpose;
	public const string InviteTokenPurpose = "Invite";
	public const string ProviderName = nameof(AuthorizationProtectorTokenProvider);
	public const string ResetPasswordTokenPurpose = UserManager<User>.ResetPasswordTokenPurpose;

	private readonly SignUpSettings _signUpSettings;


	public AuthorizationProtectorTokenProvider(
		IDataProtectionProvider dataProtectionProvider,
		IOptions<DataProtectionTokenProviderOptions> options,
		ILogger<AuthorizationProtectorTokenProvider> logger,
		IAppSettingsService appSettingsService) : base(dataProtectionProvider, options, logger)
		=> _signUpSettings = appSettingsService.GetSettings<SignUpSettings>() ?? new SignUpSettings();


	public override Task<bool> ValidateAsync(string purpose, string token, UserManager<User> manager, User user)
	{
		SetTokenLifespan(purpose);
		return base.ValidateAsync(purpose, token, manager, user);
	}


	private void SetTokenLifespan(string purpose) =>
		Options.TokenLifespan = purpose switch
		{
			ChangePhoneNumberTokenPurpose => TimeSpan.FromMinutes(_signUpSettings.ChangePhoneNumberTokenExpireTimespan),
			ConfirmEmailTokenPurpose => TimeSpan.FromMinutes(_signUpSettings.ConfirmEmailTokenExpireTimespan),
			InviteTokenPurpose => TimeSpan.FromMinutes(_signUpSettings.InviteTokenExpireTimespan),
			ResetPasswordTokenPurpose => TimeSpan.FromMinutes(_signUpSettings.ResetPasswordTokenExpireTimespan),
			_ => Options.TokenLifespan
		};
}

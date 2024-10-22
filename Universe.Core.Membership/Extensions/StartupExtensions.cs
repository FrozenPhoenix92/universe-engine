using Universe.Core.AppConfiguration.Exceptions;
using Universe.Core.Exceptions;
using Universe.Core.Membership.Authorization;
using Universe.Core.Membership.Configuration;
using Universe.Core.Membership.Data;
using Universe.Core.Membership.Model;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Universe.Core.Membership.Extensions;

/// <summary>
/// Ресширения для процесса старта приложения.
/// </summary>
public static class StartupExtensions
{
	/// <summary>
	/// Добавляет систему Identity.
	/// </summary>
	public static void AddIdentity<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext =>
		services
			.AddIdentity<User, Role>(options =>
			{
				var signUpSettings = configuration.GetSection(nameof(SignUpSettings))?.Get<SignUpSettings>() ?? new SignUpSettings();

				options.SignIn.RequireConfirmedEmail = signUpSettings.RequireConfirmedEmail;
				options.SignIn.RequireConfirmedAccount = signUpSettings.RequireApproval;

				options.Tokens.ChangeEmailTokenProvider = AuthorizationProtectorTokenProvider.ProviderName;
				options.Tokens.EmailConfirmationTokenProvider = AuthorizationProtectorTokenProvider.ProviderName;
				options.Tokens.ChangePhoneNumberTokenProvider = AuthorizationProtectorTokenProvider.ProviderName;
				options.Tokens.PasswordResetTokenProvider = AuthorizationProtectorTokenProvider.ProviderName;
			})
			.AddEntityFrameworkStores<TContext>()
			.AddDefaultTokenProviders()
			.AddUserStore<SecurityUserStore<TContext>>()
			.AddRoleStore<RoleStore<Role, TContext, Guid, UserRole, IdentityRoleClaim<Guid>>>()
			.AddUserManager<ExtendedUserManager>()
			.AddSignInManager<AuditableSignInManager>()
			.AddUserConfirmation<ApprovanceUserConfirmation>()
			.AddTokenProvider<AuthorizationProtectorTokenProvider>(AuthorizationProtectorTokenProvider.ProviderName)
			.AddPasswordValidator<RepeatingPasswordValidator>();

	public static void AddCookieAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<WebApiCookieAuthenticationEvents>();

		var section = configuration.GetSection(nameof(CookieAuthenticationSettings));
		var cookieAuthenticationConfig = section?.Get<CookieAuthenticationSettings>();
		if (section is null || cookieAuthenticationConfig is null)
			throw new MissedConfigurationSectionException(nameof(CookieAuthenticationSettings));
		services.Configure<CookieAuthenticationSettings>(section);

		services.ConfigureApplicationCookie(config =>
		{
			config.EventsType = typeof(WebApiCookieAuthenticationEvents);
			config.Cookie.Name = cookieAuthenticationConfig.CookieName;
			config.SlidingExpiration = true;
			config.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			config.ExpireTimeSpan = TimeSpan.FromMinutes(cookieAuthenticationConfig.ExpireTime <= 0
				? 30
				: cookieAuthenticationConfig.ExpireTime);
			config.LoginPath = new PathString(cookieAuthenticationConfig.LoginPath);
		});
	}
}

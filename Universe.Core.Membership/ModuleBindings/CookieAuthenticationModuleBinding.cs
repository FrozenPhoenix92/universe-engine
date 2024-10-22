using Universe.Core.AppConfiguration.Exceptions;
using Universe.Core.Membership.Authorization;
using Universe.Core.Membership.Configuration;
using Universe.Core.ModuleBinding.Bindings;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Universe.Core.Membership.ModuleBindings;

/// <summary>
/// Добавляет систему аутентификации на основе сookie.
/// </summary>
public class CookieAuthenticationModuleBinding : IAuthenticationModuleBinding
{
	public void SignUp(
		AuthenticationBuilder authBuilder,
		IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddScoped<WebApiCookieAuthenticationEvents>();

		var section = configuration.GetSection(nameof(CookieAuthenticationSettings));
		var cookieAuthenticationConfig = section.Get<CookieAuthenticationSettings>();
		if (cookieAuthenticationConfig is null)
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

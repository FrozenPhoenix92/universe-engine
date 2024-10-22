using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Universe.Core.Membership.Configuration;

namespace Universe.Core.Membership.Authorization;

internal class WebApiCookieAuthenticationEvents : CookieAuthenticationEvents
{
	private readonly CookieAuthenticationSettings _cookieAuthenticationConfig;


	public WebApiCookieAuthenticationEvents(IOptionsSnapshot<CookieAuthenticationSettings> cookieAuthenticationConfig)
	{
		_cookieAuthenticationConfig = cookieAuthenticationConfig.Value;
		OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync;
	}


	public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context) =>
		Handler(StatusCodes.Status401Unauthorized, context);

	public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context) =>
		Handler(StatusCodes.Status403Forbidden, context);


	private Task Handler(int statusCode, RedirectContext<CookieAuthenticationOptions> context)
	{
		if (_cookieAuthenticationConfig.ApiPath.Any(x => context.Request.Path.StartsWithSegments(x)))
		{
			context.Response.Headers[HeaderNames.Location] = context.RedirectUri;
			context.Response.StatusCode = statusCode;
		}
		else
		{
			context.Response.Redirect(context.RedirectUri);
		}

		return Task.CompletedTask;
	}
}

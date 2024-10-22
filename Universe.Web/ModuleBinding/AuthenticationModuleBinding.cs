using Universe.Core.AppConfiguration;
using Universe.Core.AppConfiguration.Exceptions;
using Universe.Core.ModuleBinding.Bindings;
using Universe.Web.Configuration;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using System.Text;

namespace Universe.Web.ModuleBinding;

public class AuthenticationModuleBinding : IAuthenticationModuleBinding
{
	public void SignUp(
		AuthenticationBuilder authBuilder,
		IServiceCollection services,
		IConfiguration configuration)
	{
		var clientAppJwtSettingsSection = configuration.GetSection(nameof(ClientAppJwtSettings));
		var clientAppJwtSettings = clientAppJwtSettingsSection?.Get<ClientAppJwtSettings>();
		if (clientAppJwtSettingsSection is null || clientAppJwtSettings is null)
			throw new MissedConfigurationSectionException(nameof(ClientAppJwtSettings));
		if (string.IsNullOrEmpty(clientAppJwtSettings.Audience))
			throw new MissedConfigurationSectionException(nameof(ClientAppJwtSettings.Audience));
		if (string.IsNullOrEmpty(clientAppJwtSettings.SigningKey))
			throw new MissedConfigurationSectionException(nameof(ClientAppJwtSettings.SigningKey));
		services.Configure<ClientAppJwtSettings>(clientAppJwtSettingsSection);

		authBuilder.AddJwtBearer(ProjectAuthenticationSchemes.ClientAppJwtSchemeName, config =>
		{
			config.RequireHttpsMetadata = false;
			config.SaveToken = true;
			config.TokenValidationParameters = new TokenValidationParameters
			{
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clientAppJwtSettings.SigningKey)),
				RequireExpirationTime = true,
				ValidAudience = clientAppJwtSettings.Audience,
				ValidIssuers = clientAppJwtSettings.Issuers,
				ValidateAudience = true,
				ValidateIssuer = true,
				ValidateIssuerSigningKey = true
			};
			config.Events = new JwtBearerEvents
			{
				OnMessageReceived = context =>
				{
					var accessToken = context.Request.Query["access_token"];

					var path = context.HttpContext.Request.Path;
					if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments($"/{ApiRoutes.ClientApp}client-hub"))
					{
						context.Token = accessToken;
					}
					return Task.CompletedTask;
				}
			};
		});
	}
}

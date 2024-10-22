using System.Text.Json;
using System.Text.Json.Serialization;

using Universe.Core.DependencyInjection;
using Universe.Core.Extensions;
using Universe.Core.JsonSerialization;
using Universe.Core.Middleware;
using Universe.Core.ModuleBinding;
using Universe.Core.ModuleBinding.Bindings;
using Universe.Web.Configuration;
using Universe.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Universe.Core.QueryHandling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Universe.Core.AppConfiguration.Exceptions;
using Universe.Core.Common.Extensions;
using Universe.Core.AppConfiguration;
using Universe.Web;
using Microsoft.AspNetCore.HttpOverrides;
using Universe.Core.Common.Middleware;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var configuration = BuildConfiguration(Environment);

ConfigureLogger(builder, configuration);

EnvironmentSettings? environmentSettings;

try
{
	environmentSettings = configuration.GetSection(nameof(EnvironmentSettings))?.Get<EnvironmentSettings>();
	if (environmentSettings is null)
		throw new MissedConfigurationSectionException(nameof(EnvironmentSettings));

	var clientAppJwtSettings = configuration.GetSection(nameof(ClientAppJwtSettings))?.Get<ClientAppJwtSettings>();
	if (clientAppJwtSettings is null)
		throw new MissedConfigurationSectionException(nameof(ClientAppJwtSettings));


	#region Добавление сервисов в контейнер

	var dataContextModuleBindings = AssembliesManager.GetInstances<IDataContextModuleBinding>();
	dataContextModuleBindings.ForEach(x =>
	{
		x.AddDataContext(builder.Services, configuration);
	});

	var dependenciesModuleBindings = AssembliesManager.GetInstances<IDependenciesModuleBinding>();
	dependenciesModuleBindings.ForEach(x =>
	{
		x.AddDependencies(builder.Services, configuration);
	});
	DependenciesDefaultRegistrar.AddDefaultServicesDependencies(builder.Services);

	builder.Services.AddAuthorization(config => 
	{
		config.AddPolicy(ProjectPolicies.ClientConfirmedAccountPolicyName, policy => policy.RequireClaim(ProjectClaims.ClientConfirmedAccountClaimName));
	});

	var authenticationBuilder = builder.Services.AddAuthentication();
	var authenticationBindings = AssembliesManager.GetInstances<IAuthenticationModuleBinding>();
	authenticationBindings.ForEach(bindingItem =>
	{
		try
		{
			bindingItem.SignUp(authenticationBuilder, builder.Services, configuration);
		}
		catch (Exception ex)
		{
			Log.Error(ex.Message);
		}
	});

	builder.Services
		.AddControllersWithViews(config =>
		{
			var policy = new AuthorizationPolicyBuilder()
				.RequireAuthenticatedUser()
				.Build();

			config.Filters.Add(new AuthorizeFilter(policy));
			config.Filters.Add(new ResponseCacheAttribute { NoStore = true, Location = ResponseCacheLocation.None });
			config.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

			config.ModelBinderProviders.Insert(0, new FilterInfoModelBinderProvider());
		})
		.AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
			options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

			var pureOptions = new JsonSerializerOptions(options.JsonSerializerOptions); // Options without custom converters

			options.JsonSerializerOptions.Converters.Add(new ObjectConverter());

			JsonSerializerOptionsProvider.SetOptions(pureOptions, new JsonSerializerOptions(options.JsonSerializerOptions));
		});


	if (environmentSettings.SpaClientOrigins?.Any() is true)
	{
		builder.Services.AddCors(options =>
		{
			options.AddPolicy(
				ProjectPolicies.ClientAppCorsPolicyName,
				policy => policy.AllowAnyHeader()
								.AllowAnyMethod()
								.AllowCredentials()
								.WithOrigins(environmentSettings.SpaClientOrigins)
								.WithExposedHeaders("AccessToken"));
		});
	}

	builder.Services.AddAntiforgery(options =>
	{
		options.HeaderName = "X-XSRF-TOKEN";
	});

	if (environmentSettings.SpaControlPanelSourcePath is not null)
	{
		builder.Services.AddSpaStaticFiles(options =>
		{
			options.RootPath = environmentSettings.SpaControlPanelSourcePath;
		});
	}

	builder.Services.AddObjectsMapping();

	builder.Services.AddSignalR()
		.AddJsonProtocol(o => o.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

	builder.Services.AddRateLimiter(options =>
	{
		options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

		options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
			PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
			{
				if (httpContext.Request.Path.StartsWithSegments(
					$"/{ApiRoutes.ClientApp.Substring(0, ApiRoutes.ClientApp.Length - 1)}",
					StringComparison.OrdinalIgnoreCase))
				{
					var clientKey = httpContext.GetUserId() ?? httpContext.GetUserIp() ?? string.Empty;

					return RateLimitPartition.GetFixedWindowLimiter(
						$"{clientKey} - {httpContext.Request.Path}",
						o => new FixedWindowRateLimiterOptions
						{
							AutoReplenishment = true,
							PermitLimit = 2,
							Window = TimeSpan.FromSeconds(1),
							QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
							QueueLimit = 2
						});
				}

				return RateLimitPartition.GetNoLimiter("NotClientApi");
			}),
			PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
			{
				if (httpContext.Request.Path.StartsWithSegments(
					$"/{ApiRoutes.ClientApp.Substring(0, ApiRoutes.ClientApp.Length - 1)}",
					StringComparison.OrdinalIgnoreCase))
				{
					var clientKey = httpContext.GetUserId() ?? httpContext.GetUserIp() ?? string.Empty;

					return RateLimitPartition.GetConcurrencyLimiter(
						$"{clientKey} - ConcurrencyLimiter",
						o => new ConcurrencyLimiterOptions
						{
							PermitLimit = 10,
							QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
							QueueLimit = 5
						});
				}

				return RateLimitPartition.GetNoLimiter("NotClientApi");
			}));
	
	});

	#endregion
}
catch (Exception exception)
{
	Log.Error(exception.Message);
	throw;
}


var app = builder.Build();

#region Конфигурация конвеера запроса

app.UseErrorHandlingMiddleware();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseMiddleware<IpAddressConstraintMiddleware>();

using (var serviceScope = ((IApplicationBuilder)app).ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
	serviceScope.InitializeDatabases();
	serviceScope.SeedInitialData();

	// Вызвать строго после инициализации начальных данных, в ходе которой все наборы настроек должны быть сохранены.
	serviceScope.InitializeAppConfiguration();
}

if (!IsDevelopment)
{
	app.UseHttpsRedirection();
}

app.UseRouting();

app.UseRateLimiter();

if (environmentSettings.SpaClientOrigins?.Any() is true)
	app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseAntiforgeryToken(new[] 
{ 
	$"/{ApiRoutes.ControlPanel.Substring(0, ApiRoutes.ControlPanel.Length - 1)}",
	$"/{ApiRoutes.ClientApp.Substring(0, ApiRoutes.ClientApp.Length - 1)}"
});

app.UseEndpoints(endpoints =>
{
	var signalRBindings = AssembliesManager.GetInstances<ISignalRModuleBinding>();
	signalRBindings.ForEach(o => o.MapHubs(endpoints));

	endpoints.MapControllerRoute("default", "{controller}/{action=Index}/{id?}");
});

app.UseSpa(spa =>
{
	spa.Options.SourcePath = environmentSettings.SpaControlPanelSourcePath;

	if (IsDevelopment && !string.IsNullOrEmpty(environmentSettings.SpaControlPanelProxyServer))
	{
		spa.UseProxyToSpaDevelopmentServer(environmentSettings.SpaControlPanelProxyServer);
	}
});

#endregion


await app.RunAsync();
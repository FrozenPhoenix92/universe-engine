using Serilog;

using ISerilogLogger = Serilog.ILogger;

partial class Program
{
	private static string Environment =>
		System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Development;

	private static bool IsDevelopment =>
		string.Equals(Environment, Environments.Development, StringComparison.OrdinalIgnoreCase);

	private static ISerilogLogger? Logger;


	private static IConfiguration BuildConfiguration(string environment) =>
		new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", false, true)
			.AddJsonFile($"appsettings.{environment}.json", true, true)
			.AddEnvironmentVariables()
			.Build();

	private static void ConfigureLogger(WebApplicationBuilder builder, IConfiguration configuration)
	{
		var loggerConfig = new LoggerConfiguration()
			.ReadFrom.Configuration(configuration)
			.Enrich.FromLogContext();

		if (IsDevelopment)
		{
			loggerConfig.WriteTo.Debug();
			Serilog.Debugging.SelfLog.Enable(Console.Error);
		}

		Log.Logger = loggerConfig.CreateLogger();
		Logger = Log.Logger.ForContext<Program>();

		builder.Host.UseSerilog();
	}
}
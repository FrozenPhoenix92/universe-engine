using System.Globalization;
using System.Reflection;

using Universe.Core.Configuration;
using Universe.Core.Exceptions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Universe.Core.Data;

/// <summary>
/// ������� ��������� ������, ������������ �� ����� �������������� ��������.
/// </summary>
public static class DesignTimeDataContextFactory
{
	/// <summary>
	/// ������ �������� ������ ��� ���� ���� MySQL.
	/// </summary>
	/// <typeparam name="TContext">��� ��������� ������.</typeparam>
	/// <param name="connectionStringName">�������� ��������� ������������, ����������� ������ �����������.</param>
	/// <param name="extraArguments">�������������� ���������.</param>
	/// <returns>��������� ��������� ������.</returns>
	public static TContext CreateForMySql<TContext>(string connectionStringName, params object[] extraArguments)
		where TContext : DbContext
	{
		/*if (System.Diagnostics.Debugger.IsAttached == false)
			System.Diagnostics.Debugger.Launch();*/

		var connectionString = GetConnectionString(connectionStringName);

		var builder = new DbContextOptionsBuilder<TContext>();
		builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

		var args = new List<object> { builder.Options };

		if (extraArguments is not null)
		{
			args.AddRange(extraArguments);
		}

		var result = Activator.CreateInstance(
			typeof(TContext),
			BindingFlags.CreateInstance | BindingFlags.OptionalParamBinding,
			null,
			args.ToArray(),
			CultureInfo.CurrentCulture);
		
		return result as TContext
			?? throw new StartupCriticalException($"�� ������� ������������� �������� ������ � ���� '{typeof(TContext).FullName}'.");
	}

	/// <summary>
	/// ������ �������� ������ ��� ���� ���� MySQL.
	/// </summary>
	/// <typeparam name="TContext">��� ��������� ������.</typeparam>
	/// <param name="connectionStringName">�������� ��������� ������������, ����������� ������ �����������.</param>
	/// <param name="extraArguments">�������������� ���������.</param>
	/// <returns>��������� ��������� ������.</returns>
	public static TContext CreateForPostgreSql<TContext>(string connectionStringName, params object[] extraArguments)
		where TContext : DbContext
	{
		var connectionString = GetConnectionString(connectionStringName);

		var builder = new DbContextOptionsBuilder<TContext>();
		builder.UseNpgsql(connectionString);

		var args = new List<object> { builder.Options };

		if (extraArguments is not null)
		{
			args.AddRange(extraArguments);
		}

		var result = Activator.CreateInstance(
			typeof(TContext),
			BindingFlags.CreateInstance | BindingFlags.OptionalParamBinding,
			null,
			args.ToArray(),
			CultureInfo.CurrentCulture);

		return result as TContext
			?? throw new StartupCriticalException($"�� ������� ������������� �������� ������ � ���� '{typeof(TContext).FullName}'.");
	}

	/// <summary>
	/// ������ �������� ������ ��� ���� ���� MsSQL.
	/// </summary>
	/// <typeparam name="TContext">��� ��������� ������.</typeparam>
	/// <param name="connectionStringName">�������� ��������� ������������, ����������� ������ �����������.</param>
	/// <param name="extraArguments">�������������� ���������.</param>
	/// <returns>��������� ��������� ������.</returns>
	public static TContext CreateForMsSql<TContext>(string connectionStringName, params object[] extraArguments)
		where TContext : DbContext
	{
		var connectionString = GetConnectionString(connectionStringName);

		var builder = new DbContextOptionsBuilder<TContext>();
		builder.UseSqlServer(connectionString);

		var args = new List<object> { builder.Options };

		if (extraArguments is not null)
		{
			args.AddRange(extraArguments);
		}

		var result = Activator.CreateInstance(
			typeof(TContext),
			BindingFlags.CreateInstance | BindingFlags.OptionalParamBinding,
			null,
			args.ToArray(),
			CultureInfo.CurrentCulture);

		return result as TContext
			?? throw new StartupCriticalException($"�� ������� ������������� �������� ������ � ���� '{typeof(TContext).FullName}'.");
	}

	// ����� ����� �������� ����������� �������� ���������� ������ ��� ������ ����� ��.


	private static string GetConnectionString(string connectionStringName)
	{
		var basePath = Directory.GetCurrentDirectory();
		var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

		var configuration = new ConfigurationBuilder()
		   .SetBasePath(basePath)
		   .AddJsonFile("appsettings.json")
		   .AddJsonFile($"appsettings.{environment}.json")
		   .Build();

		var connectionSettings = configuration.GetSection(connectionStringName)?.Get<DatabaseConnectionSettings>();

		if (connectionSettings is null)
			throw new MissedConfigurationSectionException(connectionStringName);

		if (string.IsNullOrWhiteSpace(connectionSettings.ConnectionString))
			throw new MissedConfigurationSectionException(
				$"{connectionStringName}.{nameof(DatabaseConnectionSettings.ConnectionString)}");

		return connectionSettings.ConnectionString;
	}
}

using Microsoft.EntityFrameworkCore;
using Universe.Core.Data;
using Universe.Core.Common.Model;
using Universe.Core.JsonSerialization;
using Universe.Core.Membership.Configuration;
using System.Text.Json;
using Universe.Core.Membership;

namespace Universe.Core.Common.ModuleBinding;

public partial class InitialDataModuleBinding
{
	private async Task SeedAppSettingsSet(IDbContext context)
	{
		var authorizationSettingsSet = await context.Set<AppSettingsSet>().SingleOrDefaultAsync(x => x.Name == nameof(AuthorizationSettings));
		if (authorizationSettingsSet is null)
		{
			await context.Set<AppSettingsSet>().AddAsync(new AppSettingsSet
			{
				Name = nameof(AuthorizationSettings),
				Value = JsonSerializer.Serialize(new AuthorizationSettings(), JsonSerializerOptionsProvider.GetOptions())
			});
		}

		var passwordSettingsSet = await context.Set<AppSettingsSet>().SingleOrDefaultAsync(x => x.Name == nameof(PasswordSettings));
		if (passwordSettingsSet is null)
		{
			await context.Set<AppSettingsSet>().AddAsync(new AppSettingsSet
			{
				AggregatedRole = AggregatedRole.Anyone,
				Name = nameof(PasswordSettings),
				Value = JsonSerializer.Serialize(new PasswordSettings(), JsonSerializerOptionsProvider.GetOptions())
			});
		}

		var signUpSettingsSet = await context.Set<AppSettingsSet>().SingleOrDefaultAsync(x => x.Name == nameof(SignUpSettings));
		if (signUpSettingsSet is null)
		{
			await context.Set<AppSettingsSet>().AddAsync(new AppSettingsSet
			{
				AggregatedRole = AggregatedRole.Anyone,
				Name = nameof(SignUpSettings),
				Value = JsonSerializer.Serialize(new SignUpSettings(), JsonSerializerOptionsProvider.GetOptions())
			});
		}

		await context.SaveChangesAsync();
	}
}
using System.Reflection;

using Universe.Core.Data;
using Universe.Core.Exceptions;
using Universe.Core.ModuleBinding.Bindings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Universe.Messaging.Model;

namespace Universe.Messaging.ModuleBindings;

public class InitialDataModuleBinding : IInitialDataModuleBinding
{
	public int Order => 80;

	public async Task EnsureInitialData(IServiceScope serviceScope)
	{
		var context = serviceScope.ServiceProvider.GetService<IDbContext>();
		if (context is null)
			throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(IDbContext));

		var appNameGlobalTemplateVariable = await context.Set<TemplateVariable>().SingleOrDefaultAsync(x => x.Name == GlobalTemplateVariableNames.AppName);
		if (appNameGlobalTemplateVariable is null)
		{
			appNameGlobalTemplateVariable = new TemplateVariable
			{
				Name = GlobalTemplateVariableNames.AppName,
				Description = "Название приложения."
			};
			await context.Set<TemplateVariable>().AddAsync(appNameGlobalTemplateVariable);
		}
		appNameGlobalTemplateVariable.Global = true;

		var currentDateGlobalTemplateVariable = await context.Set<TemplateVariable>().SingleOrDefaultAsync(x => x.Name == GlobalTemplateVariableNames.CurrentDate);
		if (currentDateGlobalTemplateVariable is null)
		{
			currentDateGlobalTemplateVariable = new TemplateVariable
			{
				Name = GlobalTemplateVariableNames.CurrentDate,
				Description = "Текущая дата."
			};
			await context.Set<TemplateVariable>().AddAsync(currentDateGlobalTemplateVariable);
		}
		currentDateGlobalTemplateVariable.Global = true;

		var currentTimeGlobalTemplateVariable = await context.Set<TemplateVariable>().SingleOrDefaultAsync(x => x.Name == GlobalTemplateVariableNames.CurrentTime);
		if (currentTimeGlobalTemplateVariable is null)
		{
			currentTimeGlobalTemplateVariable = new TemplateVariable
			{
				Name = GlobalTemplateVariableNames.CurrentTime,
				Description = "Текущее время."
			};
			await context.Set<TemplateVariable>().AddAsync(currentTimeGlobalTemplateVariable);
		}
		currentTimeGlobalTemplateVariable.Global = true;

		var userEmailGlobalTemplateVariable = await context.Set<TemplateVariable>().SingleOrDefaultAsync(x => x.Name == GlobalTemplateVariableNames.UserEmail);
		if (userEmailGlobalTemplateVariable is null)
		{
			userEmailGlobalTemplateVariable = new TemplateVariable
			{
				Name = GlobalTemplateVariableNames.UserEmail,
				Description = "Адрес электронной почты пользователя, которому предназначено письмо."
			};
			await context.Set<TemplateVariable>().AddAsync(userEmailGlobalTemplateVariable);
		}
		userEmailGlobalTemplateVariable.Global = true;

		var userNameGlobalTemplateVariable = await context.Set<TemplateVariable>().SingleOrDefaultAsync(x => x.Name == GlobalTemplateVariableNames.UserName);
		if (userNameGlobalTemplateVariable is null)
		{
			userNameGlobalTemplateVariable = new TemplateVariable
			{
				Name = GlobalTemplateVariableNames.UserName,
				Description = "Имя пользователя, которому предназначено письмо."
			};
			await context.Set<TemplateVariable>().AddAsync(userNameGlobalTemplateVariable);
		}
		userNameGlobalTemplateVariable.Global = true;

		var userSurnameGlobalTemplateVariable = await context.Set<TemplateVariable>().SingleOrDefaultAsync(x => x.Name == GlobalTemplateVariableNames.UserSurname);
		if (userSurnameGlobalTemplateVariable is null)
		{
			userSurnameGlobalTemplateVariable = new TemplateVariable
			{
				Name = GlobalTemplateVariableNames.UserSurname,
				Description = "Фамилия пользователя, которому предназначено письмо."
			};
			await context.Set<TemplateVariable>().AddAsync(userSurnameGlobalTemplateVariable);
		}
		userSurnameGlobalTemplateVariable.Global = true;

		await context.SaveChangesAsync();
	}
}

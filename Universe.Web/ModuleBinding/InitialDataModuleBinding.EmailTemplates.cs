using Microsoft.EntityFrameworkCore;
using Universe.Core.Data;
using Universe.Messaging.Model;
using System.Text;

namespace Universe.Web.ModuleBinding;

public partial class InitialDataModuleBinding
{
	private static async Task SeedEmailTemplates(IDbContext context)
	{
		var callbackUrlTemplateVariable = await CheckTemplateVariable(context, TemplateVariableNames.CallbackUrl, "Обратная ссылка.", false);
		var appNameTemplateVariable = await CheckTemplateVariable(context, TemplateVariableNames.ClientAppName, "Название приложения.", false);
		var completeRegistrationCallbackUrlTemplateVariable = await CheckTemplateVariable(
			context,
			TemplateVariableNames.CompleteRegistrationCallbackUrl,
			"Обратная ссылка на завершение регистрации клиента.",
			false);
		var registrationClientLoginTemplateVariable = await CheckTemplateVariable(
			context,
			TemplateVariableNames.RegistrationClientLogin,
			"Логин аккаунта клиента, созданного в процессе автоматической регистрации.",
			false);
		var registrationClientPasswordTemplateVariable = await CheckTemplateVariable(
			context,
			TemplateVariableNames.RegistrationClientPassword,
			"Пароль аккаунта клиента, созданного в процессе автоматической регистрации.",
			false);

		await context.SaveChangesAsync();


		var resetPasswordEmailTemplate = await context.Set<EmailTemplate>()
			.Include(x => x.EmailTemplateTemplateVariables).ThenInclude(x => x.TemplateVariable)
			.SingleOrDefaultAsync(x => x.Code == TemplateCodes.ClientResetPassword);
		if (resetPasswordEmailTemplate is null)
		{
			resetPasswordEmailTemplate = new EmailTemplate
			{
				Title = "Сброс пароля",
				Code = TemplateCodes.ClientResetPassword,
				Subject = $"Сброс пароля в приложении [{TemplateVariableNames.ClientAppName}]",
				Body = new StringBuilder()
					.AppendLine($"Для сброса старого пароля и замены его на новый пройдите по <a href='[{TemplateVariableNames.CallbackUrl}]'>ссылке</a>.<br /><br />")
					.AppendLine("Если это письмо получено по ошибке, возможно, кто-то случайно указал данный адрес электронной почты для сброса пароля.<br />")
					.AppendLine("Если это не Вы инициировали процедуру сброса пароля, просто, проигнорируйте данное письмо.")
					.ToString()
			};
			await context.Set<EmailTemplate>().AddAsync(resetPasswordEmailTemplate);
		}

		CheckEmailTemplateTemplateVariables(resetPasswordEmailTemplate, [(TemplateVariableNames.CallbackUrl, callbackUrlTemplateVariable.Id)]);

		resetPasswordEmailTemplate.System = true;

		var passwordChangedEmailTemplate = await context.Set<EmailTemplate>().SingleOrDefaultAsync(x => x.Code == TemplateCodes.ClientPasswordChanged);
		if (passwordChangedEmailTemplate is null)
		{
			passwordChangedEmailTemplate = new EmailTemplate
			{
				Title = "Изменение пароля",
				Code = TemplateCodes.ClientPasswordChanged,
				Subject = $"Изменение пароля в приложении [{TemplateVariableNames.ClientAppName}]",
				Body = new StringBuilder()
					.AppendLine("Пароль успешно изменён.")
					.ToString()
			};
			await context.Set<EmailTemplate>().AddAsync(passwordChangedEmailTemplate);
		}

		passwordChangedEmailTemplate.System = true;

		var emailConfirmedEmailTemplate = await context.Set<EmailTemplate>()
			.Include(x => x.EmailTemplateTemplateVariables).ThenInclude(x => x.TemplateVariable)
			.SingleOrDefaultAsync(x => x.Code == TemplateCodes.ClientEmailConfirmed);
		if (emailConfirmedEmailTemplate is null)
		{
			emailConfirmedEmailTemplate = new EmailTemplate
			{
				Title = "Подтверждение адреса электронной почты",
				Code = TemplateCodes.ClientEmailConfirmed,
				Subject = $"Подтверждение адреса электронной почты в приложении [{TemplateVariableNames.ClientAppName}]",
				Body = new StringBuilder()
					.AppendLine($"Для подтверждения адреса электронной почты перейдите по <a href='[{TemplateVariableNames.CallbackUrl}]'>ссылке</a>.")
					.ToString()
			};
			await context.Set<EmailTemplate>().AddAsync(emailConfirmedEmailTemplate);
		}

		CheckEmailTemplateTemplateVariables(emailConfirmedEmailTemplate, [(TemplateVariableNames.CallbackUrl, callbackUrlTemplateVariable.Id)]);

		emailConfirmedEmailTemplate.System = true;
	}


	private static void CheckEmailTemplateTemplateVariables(EmailTemplate emailTemplate, IEnumerable<(string templateVariableName, int templateVariableId)> templateVariablesData)
	{
		foreach (var item in templateVariablesData)
		{
			if (emailTemplate.EmailTemplateTemplateVariables.All(x => x.TemplateVariable?.Name != item.templateVariableName))
			{
				emailTemplate.EmailTemplateTemplateVariables.Add(new EmailTemplateTemplateVariable
				{
					TemplateVariableId = item.templateVariableId
				});
			}
		}
	}

	private static async Task<TemplateVariable> CheckTemplateVariable(IDbContext context, string templateVariableName, string templateVariableDescription, bool global)
	{
		var templateVariable = await context.Set<TemplateVariable>().SingleOrDefaultAsync(x => x.Name == templateVariableName);
			
		if (templateVariable is null)
		{
			templateVariable = new TemplateVariable
			{
				Name = templateVariableName,
				Description = templateVariableDescription
			};
			await context.Set<TemplateVariable>().AddAsync(templateVariable);
		}

		templateVariable.Global = global;

		return templateVariable;
	}
}

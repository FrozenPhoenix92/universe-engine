using Universe.Core.Membership;
using Microsoft.EntityFrameworkCore;
using Universe.Core.Data;
using Universe.Messaging.Model;
using Universe.Messaging;
using System.Text;

namespace Universe.Core.Common.ModuleBinding;

public partial class InitialDataModuleBinding
{
	private static async Task SeedEmailTemplates(IDbContext context)
	{
		var callbackUrlTemplateVariable = await context.Set<TemplateVariable>().SingleOrDefaultAsync(x => x.Name == TemplateVariableNames.CallbackUrl);
		if (callbackUrlTemplateVariable is null)
		{
			callbackUrlTemplateVariable = new TemplateVariable
			{
				Name = TemplateVariableNames.CallbackUrl,
				Description = "Обратная ссылка."
			};
			await context.Set<TemplateVariable>().AddAsync(callbackUrlTemplateVariable);
		}
		callbackUrlTemplateVariable.Global = false;

		var oldEmailTemplateVariable = await context.Set<TemplateVariable>().SingleOrDefaultAsync(x => x.Name == Membership.TemplateVariableNames.OldEmail);
		if (oldEmailTemplateVariable is null)
		{
			oldEmailTemplateVariable = new TemplateVariable
			{
				Name = Membership.TemplateVariableNames.OldEmail,
				Description = "Предыдущий адрес электронной почты."
			};
			await context.Set<TemplateVariable>().AddAsync(oldEmailTemplateVariable);
		}
		oldEmailTemplateVariable.Global = false;

		await context.SaveChangesAsync();


		var resetPasswordEmailTemplate = await context.Set<EmailTemplate>()
			.Include(x => x.EmailTemplateTemplateVariables).ThenInclude(x => x.TemplateVariable)
			.SingleOrDefaultAsync(x => x.Code == TemplateCodes.ResetPassword);
		if (resetPasswordEmailTemplate is null)
		{
			resetPasswordEmailTemplate = new EmailTemplate
			{
				Title = "Сброс пароля",
				Code = TemplateCodes.ResetPassword,
				Subject = $"Сброс пароля в приложении [{GlobalTemplateVariableNames.AppName}]",
				Body = new StringBuilder()
					.AppendLine($"Для сброса старого пароля и замены его на новый пройдите по <a href='[{TemplateVariableNames.CallbackUrl}]'>ссылке</a>.<br /><br />")
					.AppendLine("Если это письмо получено по ошибке, возможно, кто-то случайно указал данный адрес электронной почты для сброса пароля.<br />")
					.AppendLine("Если это не Вы инициировали процедуру сброса пароля, просто, проигнорируйте данное письмо.")
					.ToString()
			};
			await context.Set<EmailTemplate>().AddAsync(resetPasswordEmailTemplate);
		}
		if (resetPasswordEmailTemplate.EmailTemplateTemplateVariables.All(x => x.TemplateVariable?.Name != TemplateVariableNames.CallbackUrl))
		{
			resetPasswordEmailTemplate.EmailTemplateTemplateVariables.Add(new EmailTemplateTemplateVariable 
			{
				TemplateVariableId = callbackUrlTemplateVariable.Id
			});
		}
		resetPasswordEmailTemplate.System = true;

		var passwordChangedEmailTemplate = await context.Set<EmailTemplate>().SingleOrDefaultAsync(x => x.Code == TemplateCodes.PasswordChanged);
		if (passwordChangedEmailTemplate is null)
		{
			passwordChangedEmailTemplate = new EmailTemplate
			{
				Title = "Изменение пароля",
				Code = TemplateCodes.PasswordChanged,
				Subject = $"Изменение пароля в приложении [{GlobalTemplateVariableNames.AppName}]",
				Body = new StringBuilder()
					.AppendLine("Пароль успешно изменён.")
					.ToString()
			};
			await context.Set<EmailTemplate>().AddAsync(passwordChangedEmailTemplate);
		}
		passwordChangedEmailTemplate.System = true;

		var emailChangedEmailTemplate = await context.Set<EmailTemplate>()
			.Include(x => x.EmailTemplateTemplateVariables).ThenInclude(x => x.TemplateVariable)
			.SingleOrDefaultAsync(x => x.Code == TemplateCodes.EmailChanged);
		if (emailChangedEmailTemplate is null)
		{
			emailChangedEmailTemplate = new EmailTemplate
			{
				Title = "Изменение адреса электронной почты",
				Code = TemplateCodes.EmailChanged,
				Subject = $"Изменение адреса электронной почты в приложении [{GlobalTemplateVariableNames.AppName}]",
				Body = new StringBuilder()
					.AppendLine($"Ваш адрес электронной почты в сервисе [{GlobalTemplateVariableNames.AppName}] был изменён.<br />")
					.AppendLine($"Старый адрес электронной почты '[{Membership.TemplateVariableNames.OldEmail}]' более не используется.<br /><br />")
					.AppendLine("Если это не Вы изменили адрес электронной почты, свяжитесь с администратором сервиса как можно быстрее.")
					.ToString()
			};
			await context.Set<EmailTemplate>().AddAsync(emailChangedEmailTemplate);
		}
		if (emailChangedEmailTemplate.EmailTemplateTemplateVariables.All(x => x.TemplateVariable?.Name != TemplateVariableNames.CallbackUrl))
		{
			emailChangedEmailTemplate.EmailTemplateTemplateVariables.Add(new EmailTemplateTemplateVariable
			{
				TemplateVariableId = callbackUrlTemplateVariable.Id
			});
		}
		if (emailChangedEmailTemplate.EmailTemplateTemplateVariables.All(x => x.TemplateVariable?.Name != Membership.TemplateVariableNames.OldEmail))
		{
			emailChangedEmailTemplate.EmailTemplateTemplateVariables.Add(new EmailTemplateTemplateVariable
			{
				TemplateVariableId = oldEmailTemplateVariable.Id
			});
		}
		emailChangedEmailTemplate.System = true;

		var emailConfirmedEmailTemplate = await context.Set<EmailTemplate>()
			.Include(x => x.EmailTemplateTemplateVariables).ThenInclude(x => x.TemplateVariable)
			.SingleOrDefaultAsync(x => x.Code == TemplateCodes.EmailConfirmed);
		if (emailConfirmedEmailTemplate is null)
		{
			emailConfirmedEmailTemplate = new EmailTemplate
			{
				Title = "Подтверждение адреса электронной почты",
				Code = TemplateCodes.EmailConfirmed,
				Subject = $"Подтверждение адреса электронной почты в приложении [{GlobalTemplateVariableNames.AppName}]",
				Body = new StringBuilder()
					.AppendLine($"Для подтверждения адреса электронной почты перейдите по <a href='[{TemplateVariableNames.CallbackUrl}]'>ссылке</a>.")
					.ToString()
			};
			await context.Set<EmailTemplate>().AddAsync(emailConfirmedEmailTemplate);
		}
		if (emailConfirmedEmailTemplate.EmailTemplateTemplateVariables.All(x => x.TemplateVariable?.Name != TemplateVariableNames.CallbackUrl))
		{
			emailConfirmedEmailTemplate.EmailTemplateTemplateVariables.Add(new EmailTemplateTemplateVariable
			{
				TemplateVariableId = callbackUrlTemplateVariable.Id
			});
		}
		emailConfirmedEmailTemplate.System = true;

		var userInvitationEmailTemplate = await context.Set<EmailTemplate>()
			.Include(x => x.EmailTemplateTemplateVariables).ThenInclude(x => x.TemplateVariable)
			.SingleOrDefaultAsync(x => x.Code == TemplateCodes.UserInvitation);
		if (userInvitationEmailTemplate is null)
		{
			userInvitationEmailTemplate = new EmailTemplate
			{
				Title = "Приглашение",
				Code = TemplateCodes.UserInvitation,
				Subject = $"Приглашение от приложения [{GlobalTemplateVariableNames.AppName}]",
				Body = new StringBuilder()
					.AppendLine($"Для принятия приглашения и окончания регистрации перейдите по <a href='[{TemplateVariableNames.CallbackUrl}]'>ссылке</a>.")
					.ToString()
			};
			await context.Set<EmailTemplate>().AddAsync(userInvitationEmailTemplate);
		}
		if (userInvitationEmailTemplate.EmailTemplateTemplateVariables.All(x => x.TemplateVariable?.Name != TemplateVariableNames.CallbackUrl))
		{
			userInvitationEmailTemplate.EmailTemplateTemplateVariables.Add(new EmailTemplateTemplateVariable
			{
				TemplateVariableId = callbackUrlTemplateVariable.Id
			});
		}
		userInvitationEmailTemplate.System = true;
	}
}

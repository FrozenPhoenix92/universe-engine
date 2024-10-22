using BBWM.Messages;

using Universe.Core.AppConfiguration.Configuration;
using Universe.Core.Exceptions;
using Universe.Core.Utils;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Universe.Messaging.Services;

public class EmailMessageService : IEmailMessageService
{
	private readonly AppSettings? _appSettings;
	private readonly EmailMessagingSettings? _emailMessagingSettings;
	private readonly IEmailTemplateService? _emailTemplateService;
	private readonly IHttpContextAccessor? _httpContextAccessor;


	public EmailMessageService(
		IOptionsSnapshot<AppSettings> appSettings,
		IOptionsSnapshot<EmailMessagingSettings> emailMessagingSettings,
		IHttpContextAccessor? httpContextAccessor,
		IEmailTemplateService? emailTemplateService)
	{
		VariablesChecker.CheckIsNotNull(appSettings, nameof(appSettings));
		VariablesChecker.CheckIsNotNull(emailMessagingSettings, nameof(emailMessagingSettings));

		_appSettings = appSettings?.Value;
		_emailMessagingSettings = emailMessagingSettings?.Value;
		_emailTemplateService = emailTemplateService;
		_httpContextAccessor = httpContextAccessor;
	}


	public async Task<MailMessage> Build(string emailTemplateCode, CancellationToken ct) => await Build(emailTemplateCode, null, ct);

	public async Task<MailMessage> Build(string emailTemplateCode, IDictionary<string, string>? extraVariables = null, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNullOrEmpty(emailTemplateCode, nameof(emailTemplateCode));

		if (_emailTemplateService is null)
			throw new ConflictException("Не задан сервис управления шаблонами электронных писем.");

		var variables = GetGlobalVariables();
		if (extraVariables is not null) 
		{
			foreach (var extraVar in extraVariables)
			{
				variables[extraVar.Key] = extraVar.Value;
			}
		}

		var template = await _emailTemplateService.GetByCode(emailTemplateCode, ct);

		if (template == null)
			throw new ObjectNotExistsException($"Шаблон электронного письма с кодом '{emailTemplateCode}' не существует.");

		return new MailMessage 
		{
			Subject = SetTemplateVariables(template.Subject ?? string.Empty, variables),
			Body = SetTemplateVariables(template.Body ?? string.Empty, variables),
			IsBodyHtml = true
		};
	}
	
	public async Task Send(MailMessage mailMessage, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(mailMessage, nameof(mailMessage));


		if (_emailMessagingSettings is null)
			throw new ConflictException("Не заданы настройки для опревки электорнных писем.");

		if (string.IsNullOrEmpty(_emailMessagingSettings.Smtp))
			throw new ConflictException("Не задан SMTP сервер, необходимый для отправки электронных писем.");

		if (_emailMessagingSettings.Port is null)
			throw new ConflictException("Не задан порт SMTP сервера, необходимый для отправки электронных писем.");


		using var smtpClient = new SmtpClient(_emailMessagingSettings.Smtp, (int)_emailMessagingSettings.Port)
		{
			Credentials = new NetworkCredential(_emailMessagingSettings.UserName, _emailMessagingSettings.Password),
			EnableSsl = _emailMessagingSettings.EnableSsl
		};

		mailMessage.From ??= new MailAddress(_emailMessagingSettings.FromAddress ?? string.Empty);

		await smtpClient.SendMailAsync(mailMessage, ct);
	}


	private static string SetTemplateVariables(string templateContext, IDictionary<string, string> variables)
	{
		VariablesChecker.CheckIsNotNull(templateContext, nameof(templateContext));
		VariablesChecker.CheckIsNotNull(variables, nameof(variables));


		if (variables.Count() == 0)
			return templateContext;

		var regex = new Regex("\\[(\\w+)\\]");
		var result = templateContext;

		foreach (Match match in regex.Matches(templateContext))
		{
			var varValue = variables.SingleOrDefault(x => x.Key == match.Groups[1].Value).Value;

			if (varValue is not null)
			{
				result = result.Replace(match.Groups[0].Value, varValue);
			}
		}

		return result;
	}
	
	private IDictionary<string, string> GetGlobalVariables()
	{
		if (_httpContextAccessor?.HttpContext is null)
		{
			throw new ConflictException("Невозможно определить значения глобальных переменных для шаблона письма, " +
				"так как не задан контекст запроса.");
		}

		if (_appSettings is null)
		{
			throw new ConflictException("Невозможно определить значения глобальных переменных для шаблона письма, " +
				"так как не заданы настройки приложения.");
		}

		if (_appSettings.Name is null)
		{
			throw new ConflictException($"Невозможно определить значения глобальной переменной '{GlobalTemplateVariableNames.AppName}' " +
				"для шаблона письма, так как в конфигурации не задано название приложения."); ;
		}


		return new Dictionary<string, string>
		{
			[GlobalTemplateVariableNames.AppName] = _appSettings.Name,
			[GlobalTemplateVariableNames.UserName] = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
			[GlobalTemplateVariableNames.UserSurname] = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Surname) ?? string.Empty,
			[GlobalTemplateVariableNames.UserEmail] = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
			[GlobalTemplateVariableNames.CurrentTime] = DateTime.UtcNow.ToString()
		};
	}
}

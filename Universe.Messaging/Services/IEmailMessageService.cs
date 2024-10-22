using System.Net.Mail;

namespace Universe.Messaging.Services;

public interface IEmailMessageService
{
	Task<MailMessage> Build(string emailTemplateCode, CancellationToken ct);

	Task<MailMessage> Build(string emailTemplateCode, IDictionary<string, string>? extraVariables = null, CancellationToken ct = default);

	Task Send(MailMessage emailMessage, CancellationToken ct = default);
}

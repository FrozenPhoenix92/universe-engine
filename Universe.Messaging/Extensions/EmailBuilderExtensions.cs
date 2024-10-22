using System.Text;

namespace Universe.Messaging.Extensions;

public static class EmailBuilderExtensions
{
	public static StringBuilder AppendMailHeader(this StringBuilder body) => body
		.AppendLine($"Здравствуйте, <strong>[{GlobalTemplateVariableNames.UserName}]</strong>.")
		.AppendLine("<br /><br /><br />");

	public static StringBuilder AppendMailFooter(this StringBuilder body) => body
		.AppendLine("<br /><br /><br />")
		.AppendLine($"<span style='font-style: italic;'>Это письмо было отправлено автоматически приложением <strong>[{GlobalTemplateVariableNames.AppName}]</strong>.<br />")
		.AppendLine("Не пытайтесь ответить, так как обратный адрес не принимает письма.");

}

using Universe.Core.Infrastructure;

namespace Universe.Messaging.Dto;

/// <summary>
/// Представляет транспортый объект данных шаблона электронного письма.
/// </summary>
public class EmailTemplateDto : IDto
{
	/// <summary>
	/// Контент сообщения.
	/// </summary>
	public string? Body { get; set; }

	/// <summary>
	/// Уникальное тексовое обозначение шаблона.
	/// </summary>
	public string? Code { get; set; }

	/// <summary>
	/// Адрес электронной почты отправителя.
	/// </summary>
	public string? From { get; set; }

	/// <inheritdoc />
	public int Id { get; set; }

	/// <summary>
	/// Тема сообщения.
	/// </summary>
	public string? Subject { get; set; }

	/// <summary>
	/// Определяет, используется ли шаблон в системных операциях, в связи с чем его удаление может нарушить работу приложения.
	/// </summary>
	public bool System { get; set; }

	/// <summary>
	/// Название шаблона.
	/// </summary>
	public string? Title { get; set; }


	/// <summary>
	/// Переменные, доступные в данном шаблоне.
	/// </summary>
	public IList<EmailTemplateTemplateVariableDto> EmailTemplateTemplateVariables { get; set; } = new List<EmailTemplateTemplateVariableDto>();
}

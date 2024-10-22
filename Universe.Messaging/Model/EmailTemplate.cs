using Universe.Core.Data;

namespace Universe.Messaging.Model;

/// <summary>
/// Представляет данные шаблона электронного письма.
/// </summary>
public class EmailTemplate : IEntity
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
	public IList<EmailTemplateTemplateVariable> EmailTemplateTemplateVariables { get; set; } = new List<EmailTemplateTemplateVariable>();
}

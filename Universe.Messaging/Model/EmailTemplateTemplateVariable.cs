namespace Universe.Messaging.Model;

/// <summary>
/// Представляет связующую таблицу между шаблонами и переменными.
/// </summary>
public class EmailTemplateTemplateVariable
{
	public int EmailTemplateId { get; set; }

	public EmailTemplate? EmailTemplate { get; set; }

	public int TemplateVariableId { get; set; }

	public TemplateVariable? TemplateVariable { get; set; }
}

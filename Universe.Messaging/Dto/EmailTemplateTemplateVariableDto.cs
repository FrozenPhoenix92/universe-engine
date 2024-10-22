namespace Universe.Messaging.Dto;

public class EmailTemplateTemplateVariableDto
{
	public int EmailTemplateId { get; set; }

	public EmailTemplateDto? EmailTemplate { get; set; }

	public int TemplateVariableId { get; set; }

	public TemplateVariableDto? TemplateVariable { get; set; }
}

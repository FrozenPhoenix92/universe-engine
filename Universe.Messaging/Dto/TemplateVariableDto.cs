using Universe.Core.Infrastructure;

namespace Universe.Messaging.Dto;

/// <summary>
/// Представляет транспортный объект переменной, содержащей динамическое значение, для использования в шаблоне.
/// </summary>
public class TemplateVariableDto : IDto
{
	/// <summary>
	/// Описание переменной.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Определяет, доступно ли значение данной переменной независимо от контекста выполнения. 
	/// </summary>
	public bool Global { get; set; }

	/// <inheritdoc />
	public int Id { get; set; }

	/// <summary>
	/// Название переменной.
	/// </summary>
	public string? Name { get; set; }


	/// <summary>
	/// Шаблоны, имеющие возможность использоания данной переменной.
	/// </summary>
	public IEnumerable<EmailTemplateTemplateVariableDto> EmailTemplateTemplateVariables { get; set; } = new List<EmailTemplateTemplateVariableDto>();
}

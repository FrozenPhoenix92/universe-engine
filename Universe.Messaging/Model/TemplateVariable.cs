using Universe.Core.Data;

namespace Universe.Messaging.Model;

/// <summary>
/// Представляет переменную, содержащую динамическое значение, для использования в шаблоне.
/// </summary>
public class TemplateVariable : IEntity
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
	public IEnumerable<EmailTemplateTemplateVariable> EmailTemplateTemplateVariables { get; set; } = new List<EmailTemplateTemplateVariable>();
}

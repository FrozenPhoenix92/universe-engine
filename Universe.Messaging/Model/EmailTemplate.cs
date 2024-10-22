using Universe.Core.Data;

namespace Universe.Messaging.Model;

/// <summary>
/// ������������ ������ ������� ������������ ������.
/// </summary>
public class EmailTemplate : IEntity
{
	/// <summary>
	/// ������� ���������.
	/// </summary>
	public string? Body { get; set; }

	/// <summary>
	/// ���������� �������� ����������� �������.
	/// </summary>
	public string? Code { get; set; }

	/// <summary>
	/// ����� ����������� ����� �����������.
	/// </summary>
	public string? From { get; set; }

	/// <inheritdoc />
	public int Id { get; set; }

	/// <summary>
	/// ���� ���������.
	/// </summary>
	public string? Subject { get; set; }

	/// <summary>
	/// ����������, ������������ �� ������ � ��������� ���������, � ����� � ��� ��� �������� ����� �������� ������ ����������.
	/// </summary>
	public bool System { get; set; }

	/// <summary>
	/// �������� �������.
	/// </summary>
	public string? Title { get; set; }


	/// <summary>
	/// ����������, ��������� � ������ �������.
	/// </summary>
	public IList<EmailTemplateTemplateVariable> EmailTemplateTemplateVariables { get; set; } = new List<EmailTemplateTemplateVariable>();
}

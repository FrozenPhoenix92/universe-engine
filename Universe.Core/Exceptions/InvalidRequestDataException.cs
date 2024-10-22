namespace Universe.Core.Exceptions;

/// <summary>
/// ������������ ����� ��� ����������, ����������� ��� ���������� ��������� ������, ���������� � ���� �������.
/// </summary>
public sealed class InvalidRequestDataException : ApiException
{
	/// <summary>
	/// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.InvalidModelException"></see>.
	/// </summary>
	public InvalidRequestDataException() : base("������ ������ �������.")
	{
	}

	/// <summary>
	/// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.InvalidModelException"></see> � ���������� �� ������.
	/// </summary>
	/// <param name="message">���������, ����������� ������.</param>
	public InvalidRequestDataException(string message) : base(message)
	{
	}
}

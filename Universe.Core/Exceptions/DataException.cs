namespace Universe.Core.Exceptions;

/// <summary>
/// ������������ ����� ��� ����������, ����������� ��� ������������ ��������� ������. ������������ � ������� ������� � ���������.
/// </summary>
public class DataException : Exception
{
	/// <summary>
	/// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.DataException"></see> � ���������� �� ������.
	/// </summary>
	/// <param name="message">���������, ����������� ������.</param>
	public DataException(string message) : base(message)
	{
	}
}

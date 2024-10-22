namespace Universe.Core.Exceptions;

/// <summary>
/// ������������ ������� ����� ��� ���������� API ������. ������������ ������� ������� � ������������.
/// </summary>
public class ApiException : Exception
{
	/// <summary>
	/// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.ApiException"></see>.
	/// </summary>
	public ApiException() : base("")
    {
    }

    /// <summary>
    /// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.ApiException"></see> � ���������� �� ������.
    /// </summary>
    /// <param name="message">���������, ����������� ������.</param>
    public ApiException(string message) : base(message) 
    {
    }
}

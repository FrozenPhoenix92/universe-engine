namespace Universe.Core.Exceptions;

/// <summary>
/// ������������ ����� ��� ����������, ����������� ��� ������������ ��������� ������� �� ����� �������.
/// </summary>
public class ConflictException : Exception
{
    /// <summary>
    /// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.ConflictException"></see>.
    /// </summary>
    public ConflictException() : base("����������� �������� ����������� � ���������� � ������� ��������� �������.")
    {
    }

    /// <summary>
    /// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.ConflictException"></see> � ���������� �� ������.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ConflictException(string message) : base(message)
    {
    }
}

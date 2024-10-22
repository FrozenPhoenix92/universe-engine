namespace Universe.Core.Exceptions;

/// <summary>
/// ������������ ������� ����� ��� ����������, ����������� � ������ ������� ����������.
/// </summary>
public class StartupCriticalException : Exception
{
    /// <summary>
    /// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.StartupCriticalException"></see> � ���������� �� ������.
    /// </summary>
    /// <param name="message">���������, ����������� ������.</param>
    public StartupCriticalException(string message) : base(message)
    {
    }
}

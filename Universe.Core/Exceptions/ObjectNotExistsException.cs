namespace Universe.Core.Exceptions;

/// <summary>
/// ����������, �����������, ���� ������, ��� ������� ���������� ��������, ����.
/// </summary>
public class ObjectNotExistsException : BusinessException
{
    /// <summary>
    /// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.ObjectNotExistsException"></see>.
    /// </summary>
    public ObjectNotExistsException() : base("������ �� ����������.") 
    {
    }

    /// <summary>
    /// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.ObjectNotExistsException"></see> � ���������� �� ������.
    /// </summary>
    /// <param name="message">���������, ����������� ������.</param>
    public ObjectNotExistsException(string message) : base(message)
    {
    }
}

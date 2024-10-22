namespace Universe.Core.Exceptions;

/// <summary>
/// ������������ ������� ����� ��� ���������� ������ ������-������. ������������ ������� ������� � ��������.
/// </summary>
public class BusinessException : Exception
{
    /// <summary>
    /// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.BusinessException"></see> � ���������� �� ������.
    /// </summary>
    /// <param name="message">���������, ����������� ������.</param>
    public BusinessException(string message) : base(message) 
    {
    }
}

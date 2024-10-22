using Universe.Core.Exceptions;

namespace Universe.Core.AppConfiguration.Exceptions;

/// <summary>
/// ������������ ������� ����� ��� ����������, ����������� ��� ������������ ������������ ����������.
/// </summary>
public class ConfigurationException : ConflictException
{
    /// <summary>
    /// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.ConfigurationException"></see> � ���������� �� ������.
    /// </summary>
    /// <param name="message">���������, ����������� ������.</param>
    public ConfigurationException(string message) : base(message)
    {
    }
}

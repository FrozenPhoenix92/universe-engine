namespace Universe.Core.AppConfiguration.Exceptions;

/// <summary>
/// ������������ ����� ��� ����������, ����������� ��� ������ ������� ������������ ����������.
/// </summary>
public sealed class MissedConfigurationSectionException : ConfigurationException
{
    /// <summary>
    /// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.EmptyConfigurationSectionException"></see> � ������ ������ ������.
    /// </summary>
    /// <param name="sectionName">��� ������.</param>
    public MissedConfigurationSectionException(string sectionName)
        : base($"������������ ������ ������������ ���������� '{sectionName}' �� ������.")
    {
    }
}

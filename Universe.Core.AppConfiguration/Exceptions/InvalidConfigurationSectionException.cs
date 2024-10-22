namespace Universe.Core.AppConfiguration.Exceptions;

/// <summary>
/// ������������ ����� ��� ����������, ����������� ��� ���������� ������ � ������ ������������ ����������.
/// </summary>
public sealed class InvalidConfigurationSectionException : ConfigurationException
{
    /// <summary>
    /// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.InvalidConfigurationSectionException"></see> � ���������� �� ������.
    /// </summary>
    /// <param name="sectionName">��� ������.</param>
    public InvalidConfigurationSectionException(string sectionName)
        : base($"������������ �������� ������������ ���������� � ������ '{sectionName}'.") { }
}

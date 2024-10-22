namespace Universe.Core.AppConfiguration.Exceptions;

/// <summary>
/// Представляет класс для исключений, возникающих при пустых секциях конфигурации приложения.
/// </summary>
public sealed class MissedConfigurationSectionException : ConfigurationException
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:Universe.Core.Exceptions.EmptyConfigurationSectionException"></see> с именем пустой секции.
    /// </summary>
    /// <param name="sectionName">Имя секции.</param>
    public MissedConfigurationSectionException(string sectionName)
        : base($"Обязательная секция конфигурации приложения '{sectionName}' не задана.")
    {
    }
}

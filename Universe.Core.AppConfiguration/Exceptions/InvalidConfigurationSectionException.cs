namespace Universe.Core.AppConfiguration.Exceptions;

/// <summary>
/// Представляет класс для исключений, возникающих при невалидных данных в секции конфигурации приложения.
/// </summary>
public sealed class InvalidConfigurationSectionException : ConfigurationException
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:Universe.Core.Exceptions.InvalidConfigurationSectionException"></see> с сообщением об ошибке.
    /// </summary>
    /// <param name="sectionName">Имя секции.</param>
    public InvalidConfigurationSectionException(string sectionName)
        : base($"Недопустимое значение конфигурации приложения в секции '{sectionName}'.") { }
}

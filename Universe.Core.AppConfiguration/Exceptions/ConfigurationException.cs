using Universe.Core.Exceptions;

namespace Universe.Core.AppConfiguration.Exceptions;

/// <summary>
/// Представляет базовый класс для исключений, возникающих при неправильной конфигурации приложения.
/// </summary>
public class ConfigurationException : ConflictException
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:Universe.Core.Exceptions.ConfigurationException"></see> с сообщением об ошибке.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    public ConfigurationException(string message) : base(message)
    {
    }
}

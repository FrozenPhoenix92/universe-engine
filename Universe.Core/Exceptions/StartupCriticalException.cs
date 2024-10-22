namespace Universe.Core.Exceptions;

/// <summary>
/// Представляет базовый класс для исключений, возникающих в период запуска приложения.
/// </summary>
public class StartupCriticalException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:Universe.Core.Exceptions.StartupCriticalException"></see> с сообщением об ошибке.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    public StartupCriticalException(string message) : base(message)
    {
    }
}

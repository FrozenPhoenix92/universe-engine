namespace Universe.Core.Exceptions;

/// <summary>
/// Представляет класс для исключений, возникающих при некорректном состоянии системы во время запроса.
/// </summary>
public class ConflictException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:Universe.Core.Exceptions.ConflictException"></see>.
    /// </summary>
    public ConflictException() : base("Выполняемая операция столкнулась с конфликтом в текущем состояния системы.")
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:Universe.Core.Exceptions.ConflictException"></see> с сообщением об ошибке.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ConflictException(string message) : base(message)
    {
    }
}

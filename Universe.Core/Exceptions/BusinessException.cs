namespace Universe.Core.Exceptions;

/// <summary>
/// Представляет базовый класс для исключений уровня бизнес-логики. Используется главным образом в сервисах.
/// </summary>
public class BusinessException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:Universe.Core.Exceptions.BusinessException"></see> с сообщением об ошибке.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    public BusinessException(string message) : base(message) 
    {
    }
}

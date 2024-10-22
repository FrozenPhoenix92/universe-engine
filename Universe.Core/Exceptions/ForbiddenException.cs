namespace Universe.Core.Exceptions;

/// <summary>
/// Исключение, возникающее при попытке выполнить запрещённую операцию.
/// </summary>
public sealed class ForbiddenException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:Universe.Core.Exceptions.ForbiddenException"></see>.
    /// </summary>
    public ForbiddenException() : base("Доступ запрещён.") { }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:Universe.Core.Exceptions.ForbiddenException"></see> с сообщением об ошибке.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    public ForbiddenException(string message) : base(message) { }
}

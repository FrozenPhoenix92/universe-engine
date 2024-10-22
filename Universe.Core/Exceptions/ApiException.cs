namespace Universe.Core.Exceptions;

/// <summary>
/// Представляет базовый класс для исключений API уровня. Используется главным образом в контроллерах.
/// </summary>
public class ApiException : Exception
{
	/// <summary>
	/// Инициализирует новый экземпляр класса <see cref="T:Universe.Core.Exceptions.ApiException"></see>.
	/// </summary>
	public ApiException() : base("")
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:Universe.Core.Exceptions.ApiException"></see> с сообщением об ошибке.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    public ApiException(string message) : base(message) 
    {
    }
}

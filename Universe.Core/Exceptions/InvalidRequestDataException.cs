namespace Universe.Core.Exceptions;

/// <summary>
/// Представляет класс для исключений, возникающих при невалидном состоянии модели, переданной в теле запроса.
/// </summary>
public sealed class InvalidRequestDataException : ApiException
{
	/// <summary>
	/// Инициализирует новый экземпляр класса <see cref="T:Universe.Core.Exceptions.InvalidModelException"></see>.
	/// </summary>
	public InvalidRequestDataException() : base("Ошибка данных запроса.")
	{
	}

	/// <summary>
	/// Инициализирует новый экземпляр класса <see cref="T:Universe.Core.Exceptions.InvalidModelException"></see> с сообщением об ошибке.
	/// </summary>
	/// <param name="message">Сообщение, описывающее ошибку.</param>
	public InvalidRequestDataException(string message) : base(message)
	{
	}
}

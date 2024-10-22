namespace Universe.Core.Exceptions;

/// <summary>
/// Представляет класс для исключений, возникающих при некорректном состоянии данных. Используется в классах моделей и контекста.
/// </summary>
public class DataException : Exception
{
	/// <summary>
	/// Инициализирует новый экземпляр класса <see cref="T:Universe.Core.Exceptions.DataException"></see> с сообщением об ошибке.
	/// </summary>
	/// <param name="message">Сообщение, описывающее ошибку.</param>
	public DataException(string message) : base(message)
	{
	}
}

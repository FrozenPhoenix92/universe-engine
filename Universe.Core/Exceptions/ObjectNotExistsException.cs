namespace Universe.Core.Exceptions;

/// <summary>
/// »сключение, возникающее, если объект, над которым проводитс€ операци€, пуст.
/// </summary>
public class ObjectNotExistsException : BusinessException
{
    /// <summary>
    /// »нициализирует новый экземпл€р класса <see cref="T:Universe.Core.Exceptions.ObjectNotExistsException"></see>.
    /// </summary>
    public ObjectNotExistsException() : base("ќбъект не существует.") 
    {
    }

    /// <summary>
    /// »нициализирует новый экземпл€р класса <see cref="T:Universe.Core.Exceptions.ObjectNotExistsException"></see> с сообщением об ошибке.
    /// </summary>
    /// <param name="message">—ообщение, описывающее ошибку.</param>
    public ObjectNotExistsException(string message) : base(message)
    {
    }
}

namespace Universe.Core.Exceptions;

/// <summary>
/// »сключение, возникающее при обращении к несуществующему ресурсу.
/// </summary>
public sealed class EntityNotFoundException : ApiException
{
    /// <summary>
    /// »нициализирует новый экземпл€р класса <see cref="T:Universe.Core.Exceptions.EntityNotFoundException"></see>.
    /// </summary>
    public EntityNotFoundException() : base("–есурс не найден.")
    {
    }

    /// <summary>
    /// »нициализирует новый экземпл€р класса <see cref="T:Universe.Core.Exceptions.EntityNotFoundException"></see> с сообщением об ошибке.
    /// </summary>
    /// <param name="message">—ообщение, описывающее ошибку.</param>
    public EntityNotFoundException(string message) : base(message)
    {
    }
}

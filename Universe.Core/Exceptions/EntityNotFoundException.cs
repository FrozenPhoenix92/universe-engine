namespace Universe.Core.Exceptions;

/// <summary>
/// ����������, ����������� ��� ��������� � ��������������� �������.
/// </summary>
public sealed class EntityNotFoundException : ApiException
{
    /// <summary>
    /// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.EntityNotFoundException"></see>.
    /// </summary>
    public EntityNotFoundException() : base("������ �� ������.")
    {
    }

    /// <summary>
    /// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.EntityNotFoundException"></see> � ���������� �� ������.
    /// </summary>
    /// <param name="message">���������, ����������� ������.</param>
    public EntityNotFoundException(string message) : base(message)
    {
    }
}

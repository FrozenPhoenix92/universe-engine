namespace Universe.Core.Exceptions;

/// <summary>
/// ����������, ����������� ��� ������� ��������� ����������� ��������.
/// </summary>
public sealed class ForbiddenException : Exception
{
    /// <summary>
    /// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.ForbiddenException"></see>.
    /// </summary>
    public ForbiddenException() : base("������ ��������.") { }

    /// <summary>
    /// �������������� ����� ��������� ������ <see cref="T:Universe.Core.Exceptions.ForbiddenException"></see> � ���������� �� ������.
    /// </summary>
    /// <param name="message">���������, ����������� ������.</param>
    public ForbiddenException(string message) : base(message) { }
}

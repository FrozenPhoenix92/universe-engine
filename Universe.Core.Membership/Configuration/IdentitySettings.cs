namespace Universe.Core.Membership.Configuration;

/// <summary>
/// Представляет набор настроек, определяющих правила идентичности.
/// </summary>
public class IdentitySettings
{
    /// <summary>
    /// Определяет, должно ли имя пользователя и адрес электронной почты быть единым параметром с одинаковым значением.
    /// </summary>
    public bool SameUserNameEmail { get; set; } = true;
}

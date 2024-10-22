using Universe.Core.Data;

namespace Universe.Core.Membership.Model;

public enum LoginAuditResult
{
	Success,
	Failure,
	AuthenticatorCodeRequired
}

public class SignInAudit : IEntity
{
    public int Id { get; set; }

    public DateTime DateTime { get; set; }

    public string? IdentityIdentifier { get; set; }

    public string? Ip { get; set; }

    public string? Fingerprint { get; set; }

    public string? Browser { get; set; }

    public LoginAuditResult? Result { get; set; }

    public string? Message { get; set; }
}

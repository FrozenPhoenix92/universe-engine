using Universe.Core.Infrastructure;
using Universe.Core.Membership.Model;

namespace Universe.Core.Membership.Dto;

public class SignInAuditDto : IDto
{
    public int Id { get; set; }

    public DateTime DateTime { get; set; }

    public string? Ip { get; set; }

    public string? Fingerprint { get; set; }

    public string? Email { get; set; }

    public string? Browser { get; set; }

    public LoginAuditResult? Result { get; set; }

    public string? Message { get; set; }
}

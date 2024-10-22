namespace Universe.Core.Membership.Dto;

public class SignInResponse
{
    public bool TwoFactorCodeRequired { get; set; }

    public SignInResponseSessionData? SessionData { get; set; }
}

public class SignInResponseSessionData
{
    public Guid UserId { get; set; }

    public string? UserName { get; set; }

    public IEnumerable<string> Roles { get; set; } = new List<string>();

    public IEnumerable<string> Permissions { get; set; } = new List<string>();
}

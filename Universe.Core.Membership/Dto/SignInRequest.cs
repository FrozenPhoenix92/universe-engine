namespace Universe.Core.Membership.Dto;

public class SignInRequest
{
    public string? Login { get; set; }

    public string? Password { get; set; }

    public string? CaptchaResponse { get; set; }

    public string? Fingerprint { get; set; }

    public string? Browser { get; set; }

    public string? TwoFactorCode { get; set; }

    public string? TwoFactorRecoveryCode { get; set; }
}

namespace Universe.Web.Services;

public interface IReCaptchaService
{
	Task<bool> ValidateResponse(string userResponse, string appCode, CancellationToken ct = default);
}

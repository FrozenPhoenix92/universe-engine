using Universe.Core.Exceptions;
using Universe.Core.Extensions;
using Universe.Core.Membership.Model;
using Universe.Core.Membership.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Security.Claims;

using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace Universe.Core.Membership.Authorization;

public class AuditableSignInManager : SignInManager<User>
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ISignInAuditService _signInAuditService;


    public AuditableSignInManager(
        IAuthenticationSchemeProvider authenticationSchemeProvider,
        IUserClaimsPrincipalFactory<User> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        IUserConfirmation<User> userConfirmation,
        ILogger<AuditableSignInManager> logger,
        ISignInAuditService signInAuditService,
        IHttpContextAccessor contextAccessor,
        UserManager<User> userManager) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, authenticationSchemeProvider, userConfirmation)
    {
        _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        _signInAuditService = signInAuditService ?? throw new ArgumentNullException(nameof(signInAuditService));
    }


    public override async Task<SignInResult> PasswordSignInAsync(User user, string password, bool isPersistent, bool lockoutOnFailure)
    {
        var result = await base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);

        var signInAudit = CreateSignInAuditFromHttpContext(_contextAccessor.HttpContext);
        signInAudit.IdentityIdentifier = _contextAccessor?.HttpContext?.GetUserId();

        if (result.IsNotAllowed)
        {
            signInAudit.Result = LoginAuditResult.Failure;
            signInAudit.Message = "����������� �������������� ����������.";
        }

        if (result.IsLockedOut)
        {
            signInAudit.Result = LoginAuditResult.Failure;
            signInAudit.Message = "������������ ������������.";
        }

        if (result.RequiresTwoFactor)
        {
            signInAudit.Result = LoginAuditResult.AuthenticatorCodeRequired;
            signInAudit.Message = "��������� ������ �������� ������������� ��������������.";
        }

        if (result.Succeeded)
        {
            signInAudit.Result = LoginAuditResult.Success;
            signInAudit.Message = "�������� �������������� � ������� ������ � ������.";
        }

        await _signInAuditService.Create(signInAudit);

        return result;
    }

    public override async Task SignInWithClaimsAsync(User user, AuthenticationProperties? authenticationProperties, IEnumerable<Claim> additionalClaims)
    {
        await base.SignInWithClaimsAsync(user, authenticationProperties, additionalClaims);

        var signInAudit = CreateSignInAuditFromHttpContext(_contextAccessor.HttpContext);
        var methodName = additionalClaims.SingleOrDefault(x => x.Type == ClaimTypes.AuthenticationMethod)?.Value;
        signInAudit.IdentityIdentifier = _contextAccessor?.HttpContext?.GetUserId();
        signInAudit.Result = LoginAuditResult.Success;
        signInAudit.Message = methodName is null
            ? "�������� ��������������."
            : $"�������� �������������� � ������� ������ '{methodName}'.";

        await _signInAuditService.Create(signInAudit);
    }

    public override async Task SignOutAsync()
    {
        var signInAudit = CreateSignInAuditFromHttpContext(_contextAccessor?.HttpContext);
        signInAudit.IdentityIdentifier = _contextAccessor?.HttpContext?.GetUserId();

        await base.SignOutAsync();

        signInAudit.Result = LoginAuditResult.Success;
        signInAudit.Message = "����� �� �������.";

        await _signInAuditService.Create(signInAudit);
    }

    public override async Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string code, bool isPersistent, bool rememberClient)
    {
        var signInAudit = CreateSignInAuditFromHttpContext(_contextAccessor?.HttpContext);

        var result = await base.TwoFactorAuthenticatorSignInAsync(code, isPersistent, rememberClient);

        signInAudit.IdentityIdentifier = _contextAccessor?.HttpContext?.GetUserId();

        if (result.Succeeded)
        {
            signInAudit.Result = LoginAuditResult.Success;
            signInAudit.Message = "�������� ����������� ������������� ��������������.";
        }
        else
        {
            signInAudit.Result = LoginAuditResult.Failure;
            signInAudit.Message = "��������� ����������� ������������� ��������������.";
        }

        return result;
    }


    private static SignInAudit CreateSignInAuditFromHttpContext(HttpContext? httpContext)
    {
        if (httpContext is null)
            throw new ConflictException("���������� �������� �������.");

        var browserId = httpContext.Request.Headers["X-Browser-Id"];
        if (string.IsNullOrEmpty(browserId))
            browserId = httpContext.Request.Headers["User-Agent"];

        return new SignInAudit
        {
            DateTime = DateTime.Now,
            Ip = httpContext.GetUserIp(),
            Browser = browserId,
            Fingerprint = httpContext.Request.Headers["X-Browser-Fingerprint"],
        };
    }
}
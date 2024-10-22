using Universe.Core.Membership.Authorization;
using Universe.Core.Membership;
using Universe.Messaging.Api;

namespace Universe.Web;

public static class ProjectAuthenticationSchemes
{
	public const string ClientAppJwtSchemeName = "ClientAppJwt";
	public const string UniversalSchemeName = "Universal";
}

public static class ProjectClaims
{
	public const string ClientConfirmedAccountClaimName = "ClientAccountConfirmed";
}

public static class ProjectConstants
{
}

public class ProjectPermissions : ISuperAdminPermissions
{
	public const string ManageClient = $"{CorePolicies.PermissionPolicyName}ProjectManageClient";
}

public static class ProjectPolicies
{
	public const string ClientAppCorsPolicyName = "ClientApplicationCors";
	public const string ClientConfirmedAccountPolicyName = "ClientConfirmedAccount";
}

public class ProjectRoles : IRoleProvider
{
	public const string AdminRoleName = "Admin";
}

public class ProjectRolePermissions : IRolePermissionProvider
{
	public IDictionary<string, IEnumerable<string>> RolesPermissionsMap => new Dictionary<string, IEnumerable<string>>
	{
		[ProjectRoles.AdminRoleName] = new List<string> 
		{
			ProjectPermissions.ManageClient,
			ModulePermissions.ManageEmailTemplatePermissionName,
			CorePermissions.ManageAppConfigurationPermissionName,
			CorePermissions.ManageDataChangeLogPermissionName,
			CorePermissions.ManageLoginAuditPermissionName,
			CorePermissions.ManagePermissionPermissionName,
			CorePermissions.ManageRolePermissionName,
			CorePermissions.ManageUserPermissionName
		}
	};
}

public static class TemplateCodes
{
	public const string ClientResetPassword = "ClientResetPassword";
	public const string ClientPasswordChanged = "ClientPasswordChanged";
	public const string ClientEmailConfirmed = "ClientEmailConfirmed";
	public const string ConfirmedClientExchangeOrderCreated = "ConfirmedClientExchangeOrderCreated";
}

public static class TemplateVariableNames
{
	public const string ClientAppName = "ClientAppName";
	public const string CallbackUrl = "CallbackUrl";
	public const string CompleteRegistrationCallbackUrl = "CompleteRegistrationCallbackUrl";
	public const string RegistrationClientLogin = "RegistrationClientLogin";
	public const string RegistrationClientPassword = "RegistrationClientPassword";
	public const string SignInCallbackUrl = "SignInCallbackUrl";
}

public static class ProjectHeaderNames
{
}


using System.Runtime.Serialization;

using Universe.Core.Membership.Authorization;

namespace Universe.Core.Membership;

public enum AggregatedRole
{
	[EnumMember(Value = "Authenticated")] Authenticated = 0,
	[EnumMember(Value = "Anyone")] Anyone = 1,
	[EnumMember(Value = "Noone")] Noone = 2
}


public class CorePermissions : ISuperAdminPermissions
{
	public const string ManageAppConfigurationPermissionName = $"{CorePolicies.PermissionPolicyName}CoreManageAppConfiguration";
	public const string ManageDataChangeLogPermissionName = $"{CorePolicies.PermissionPolicyName}CoreManageDataChangeLog";
	public const string ManageLoginAuditPermissionName = $"{CorePolicies.PermissionPolicyName}CoreManageLoginAudit";
	public const string ManagePermissionPermissionName = $"{CorePolicies.PermissionPolicyName}CoreManagePermission";
	public const string ManageRolePermissionName = $"{CorePolicies.PermissionPolicyName}CoreManageRole";
	public const string ManageUserPermissionName = $"{CorePolicies.PermissionPolicyName}CoreManageUser";
	public const string ReadAuditDataPermissionName = $"{CorePolicies.PermissionPolicyName}CoreReadAuditData";
}

public static class CorePolicies
{
	public const string PermissionPolicyName = "Permission";
	public const string TwoFactorRequiredRolePolicyName = "TwoFactorRequired";
}

public static class CoreRoles
{
	public const string SuperAdminRoleName = "SuperAdmin";
}

public static class TemplateCodes
{
	public const string ResetPassword = "ResetPassword";
	public const string PasswordChanged = "PasswordChanged";
	public const string EmailChanged = "EmailChanged";
	public const string EmailConfirmed = "EmailConfirmed";
	public const string UserInvitation = "UserInvitation";
}

public static class TemplateVariableNames
{
	public const string CallbackUrl = "CallbackUrl";
	public const string OldEmail = "OldEmail";
}

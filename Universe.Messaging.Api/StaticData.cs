using Universe.Core.Membership.Authorization;
using Universe.Core.Membership;

namespace Universe.Messaging.Api;

public class ModulePermissions : ISuperAdminPermissions
{
	public const string ManageEmailTemplatePermissionName = $"{CorePolicies.PermissionPolicyName}ModuleManageEmailTemplate";
}
export class AggregatedRoles {
	static readonly Anyone = "Anyone";
	static readonly Authorized = "Authorized";
}

export class CorePermissions {
	static readonly ManageAppConfiguration = "PermissionCoreManageAppConfiguration";
	static readonly ManageDataChangeLog = "PermissionCoreManageDataChangeLog";
	static readonly ManageEmailTemplate = "PermissionModuleManageEmailTemplate";
	static readonly ManageLoginAudit = "PermissionCoreManageLoginAudit";
	static readonly ManagePermission = "PermissionCoreManagePermission";
	static readonly ManageRole = "PermissionCoreManageRole";
	static readonly ManageUser = "PermissionCoreManageUser";
	static readonly ReadAuditData = "PermissionCoreReadAuditData";
}

export class CoreRoles {
	static readonly SuperAdmin = "SuperAdmin";
}
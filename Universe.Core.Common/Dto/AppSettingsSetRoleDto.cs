using Universe.Core.Membership.Dto;

namespace Universe.Core.Common.Dto;

public class AppSettingsSetRoleDto
{
	public string? RoleId { get; set; }

	public RoleDto? Role { get; set; }

	public int AppSettingsSetId { get; set; }

	public AppSettingsSetDto? AppSettingsSet { get; set; }

	public bool AllowChange { get; set; }
}

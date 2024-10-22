using Universe.Core.Membership.Dto;

namespace Universe.Core.Common.Dto;

public class AppSettingsSetPermissionDto
{
	public string? PermissionId { get; set; }

	public PermissionDto? Permission { get; set; }

	public int AppSettingsSetId { get; set; }

	public AppSettingsSetDto? AppSettingsSet { get; set; }

	public bool AllowChange { get; set; }
}

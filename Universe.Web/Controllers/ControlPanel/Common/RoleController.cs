using AutoMapper;

using Universe.Core.AppConfiguration;
using Universe.Core.Infrastructure;
using Universe.Core.Membership;
using Universe.Core.Membership.Dto;
using Universe.Core.Membership.Model;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Universe.Web.Controllers.ControlPanel.Common;

[Route($"{ApiRoutes.ControlPanel}role")]
[Authorize(Policy = CorePermissions.ManageRolePermissionName)]
public class RoleController : DataControllerCore<Role, RoleDto, Guid>
{
	public RoleController(IMapper mapper, IDataService<Role, Guid> defaultDataOperationsService)
		: base(mapper, defaultDataOperationsService)
	{
	}
}

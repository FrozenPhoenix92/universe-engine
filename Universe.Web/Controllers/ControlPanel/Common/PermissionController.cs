using AutoMapper;

using Universe.Core.AppConfiguration;
using Universe.Core.Infrastructure;
using Universe.Core.Membership;
using Universe.Core.Membership.Dto;
using Universe.Core.Membership.Model;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Universe.Web.Controllers.ControlPanel.Common;

[Route($"{ApiRoutes.ControlPanel}permission")]
[Authorize(Policy = CorePermissions.ManagePermissionPermissionName)]
public class PermissionController : DataControllerCore<Permission, PermissionDto, Guid>
{
	public PermissionController(IMapper mapper, IDataService<Permission, Guid> defaultDataOperationsService)
		: base(mapper, defaultDataOperationsService)
	{
	}
}

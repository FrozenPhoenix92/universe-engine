using AutoMapper;

using Universe.Core.AppConfiguration;
using Universe.Core.Audit;
using Universe.Core.Infrastructure;
using Universe.Core.Membership;
using Universe.Core.Membership.Dto;
using Universe.Core.Membership.Model;
using Universe.Core.QueryHandling;
using Universe.Core.Utils;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Universe.Core.Common.Controllers;

[Route($"{ApiRoutes.ControlPanel}sign-in-audit")]
[Authorize(Policy = CorePermissions.ReadAuditDataPermissionName)]
public class SignInAuditController : ControllerBase
{
	private readonly IMapper _mapper;


	public SignInAuditController(IMapper mapper)
	{
		VariablesChecker.CheckIsNotNull(mapper, nameof(mapper));

		_mapper = mapper;
	}


	[HttpGet]
	public async Task<IActionResult> GetAll(
		[FromQuery] QueryCommand command,
		[FromServices] IDataService<SignInAudit, int, IAuditDataContext> dataService,
		CancellationToken ct = default)
		=> Ok(_mapper.Map<IEnumerable<SignInAuditDto>>(await dataService.GetAll(command, ct)));

	[HttpGet("total")]
	public async Task<IActionResult> GetTotal(
		[FromQuery] QueryCommand command,
		[FromServices] IDataService<SignInAudit, int, IAuditDataContext> dataService,
		CancellationToken ct = default)
		=> Ok(await dataService.GetTotal(command, ct));
}

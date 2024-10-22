using AutoMapper;

using Universe.Core.AppConfiguration;
using Universe.Core.Audit;
using Universe.Core.Infrastructure;
using Universe.Core.Membership;
using Universe.Core.QueryHandling;
using Universe.Core.Utils;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Universe.Core.Common.Controllers;

[Route($"{ApiRoutes.ControlPanel}data-audit")]
[Authorize(Policy = CorePermissions.ReadAuditDataPermissionName)]
public class DataAuditController : ControllerBase
{
	private readonly IMapper _mapper;


	public DataAuditController(IMapper mapper)
	{
		VariablesChecker.CheckIsNotNull(mapper, nameof(mapper));

		_mapper = mapper;
	}


	[HttpGet]
	public async Task<IActionResult> GetAll(
		[FromQuery] QueryCommand command,
		[FromServices] IDataService<ChangeLog, int, IAuditDataContext> dataService,
		CancellationToken ct = default)
		=> Ok(_mapper.Map<IEnumerable<ChangeLogDto>>(
			await dataService.GetAll(command, query => query.Include(x => x.ChangeLogItems), ct)));

	[HttpGet("total")]
	public async Task<IActionResult> GetTotal(
		[FromQuery] QueryCommand command,
		[FromServices] IDataService<ChangeLog, int, IAuditDataContext> dataService,
		CancellationToken ct = default)
		=> Ok(await dataService.GetTotal(command, ct));
}

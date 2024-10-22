using AutoMapper;
using Universe.Core.AppConfiguration;
using Universe.Core.Data;
using Universe.Core.Infrastructure;
using Universe.Messaging.Dto;
using Universe.Messaging.Model;
using Universe.Messaging.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Universe.Messaging.Api;

[Route($"{ApiRoutes.ControlPanel}email-template")]
[Authorize(Policy = ModulePermissions.ManageEmailTemplatePermissionName)]
public class EmailTemplateController : DataControllerCore<EmailTemplate, EmailTemplateDto>
{
	public EmailTemplateController(
		IMapper mapper,
		IDataService<EmailTemplate, int, IDbContext> defaultDataOperationsService,
		IEmailTemplateService emailTemplateService)
		: base(mapper, defaultDataOperationsService, emailTemplateService)
	{
	}


	public async Task<IActionResult> GetGlobalVariables(CancellationToken ct) =>
		Ok(await GetCustomService<IEmailTemplateService>().GetGlobalVariables(ct));
}

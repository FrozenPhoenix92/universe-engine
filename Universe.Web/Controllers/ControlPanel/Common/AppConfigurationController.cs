using AutoMapper;
using Universe.Core.AppConfiguration;
using Universe.Core.Common.Dto;
using Universe.Core.Common.Model;
using Universe.Core.Common.Services;
using Universe.Core.Infrastructure;
using Universe.Core.Membership;
using Universe.Core.Utils;
using Universe.Files.Configuration;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Universe.Web.Controllers.ControlPanel.Common;

[Route($"{ApiRoutes.ControlPanel}app-configuration")]
public class AppConfigurationController : ControllerCore
{
	private const string ControlPanelEnvironmentSettingsSetName = "ControlPanelEnvironmentSettings";


	private readonly IAppSettingsDataService _appSettingsDataService;
	private readonly FileStorageSettings _fileStorageSettings;


	public AppConfigurationController(
		IMapper mapper,
		IAppSettingsDataService appSettingsDataService,
		IOptions<FileStorageSettings> fileStorageSettingsOptions) : base(mapper)
	{
		VariablesChecker.CheckIsNotNull(appSettingsDataService, nameof(appSettingsDataService));
		VariablesChecker.CheckIsNotNull(fileStorageSettingsOptions, nameof(fileStorageSettingsOptions));

		_appSettingsDataService = appSettingsDataService;
		_fileStorageSettings = fileStorageSettingsOptions.Value;
	}

	[AllowAnonymous]
	[HttpGet]
	public async Task<IActionResult> Get(CancellationToken ct)
		=> Ok(Mapper.Map<IEnumerable<AppSettingsSetDto>>(await _appSettingsDataService.GetAllowedForCurrentUserSettings(ct)));

	[HttpPost]
    [Authorize(Policy = CorePermissions.ManageAppConfigurationPermissionName)]
    public async Task<IActionResult> Save([FromBody] AppSettingsSet appSettingsSet, CancellationToken ct) =>
        Ok(Mapper.Map<AppSettingsSetDto>(await _appSettingsDataService.SaveSettings(appSettingsSet, ct)));
}

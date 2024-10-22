using AutoMapper;

using Universe.Core.AppConfiguration;
using Universe.Core.Exceptions;
using Universe.Core.Infrastructure;
using Universe.Core.Membership;
using Universe.Core.Membership.Configuration;
using Universe.Core.Membership.Dto;
using Universe.Core.Membership.Model;
using Universe.Core.Membership.Services;
using Universe.Core.Utils;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using System.ComponentModel.DataAnnotations;

namespace Universe.Web.Controllers.ControlPanel.Common;

[Route($"{ApiRoutes.ControlPanel}user")]
[Authorize(Policy = CorePermissions.ManageUserPermissionName)]
public class UserController : DataControllerCore<User, UserDto, Guid>
{
	private readonly IdentitySettings _identitySettings;
	private readonly RoleManager<Role> _roleManager;


	public UserController(IMapper mapper,
		                  IDataService<User, Guid> defaultDataOperationsService,
	                      IUserService customDataOperationsService,
	                      IOptionsSnapshot<IdentitySettings> identitySettingsOptions,
	                      RoleManager<Role> roleManager)
		: base(mapper, defaultDataOperationsService, customDataOperationsService)
	{
		VariablesChecker.CheckIsNotNull(identitySettingsOptions, nameof(identitySettingsOptions));
		VariablesChecker.CheckIsNotNull(identitySettingsOptions.Value, nameof(identitySettingsOptions.Value));
		VariablesChecker.CheckIsNotNull(roleManager, nameof(roleManager));

		_identitySettings = identitySettingsOptions.Value;
		_roleManager = roleManager;
	}


	[HttpPost("init-super-admin")]
	[AllowAnonymous]
	public async Task<IActionResult> InitSuperAdmin([FromBody] UserDto dto, CancellationToken ct)
		=> await NoContent(async () => 
		{
			if (AppState.SuperAdminExists)
				throw new ForbiddenException("Пользователь с ролью суперадминистратора уже существует.");

			if (string.IsNullOrWhiteSpace(dto.Password))
				throw new ValidationException("Для создания суперадмина необходимо задать пароль.");

			var superAdminRole = await _roleManager.FindByNameAsync(CoreRoles.SuperAdminRoleName);

			if (superAdminRole is null)
				throw new ConflictException("Роль суперадминистратора не существует.");

			if (_identitySettings.SameUserNameEmail)
				dto.Email = dto.UserName;

			var user = Mapper.Map<User>(dto);
			user.Approved = true;
			user.EmailConfirmed = true;
			user.PhoneNumberConfirmed = true;
			user.UserPermissions = new List<UserPermission>();
			user.RootAdmin = true;
			user.UserRoles = new List<UserRole>
			{
				new UserRole
				{
					RoleId = superAdminRole.Id
				}
			};

			await GetCustomService<IUserService>().Validate(dto, user, ct);
			await GetCustomService<IUserService>().Create(user, dto.Password, ct);

			AppState.SuperAdminExists = true;
		});

	[HttpGet("super-admin-exists")]
	[AllowAnonymous]
	public IActionResult SuperAdminExists() => Ok(AppState.SuperAdminExists);
}

using Universe.Core.AppConfiguration.Services;
using Universe.Core.Data;
using Universe.Core.Exceptions;
using Universe.Core.Infrastructure;
using Universe.Core.Membership.Authorization;
using Universe.Core.Membership.Configuration;
using Universe.Core.Membership.Dto;
using Universe.Core.Membership.Model;
using Universe.Core.Utils;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;

using System.ComponentModel.DataAnnotations;

namespace Universe.Core.Membership.Services;

public class UserService : IUserService
{
	private readonly IAppSettingsService _appSettingsService;
	private readonly IDbContext _dataContext;
	private readonly IDataService<User, Guid> _dataService;
	private readonly IdentitySettings _identitySettings;
	private readonly PermissionManager _permissionManager;
	private readonly RoleManager<Role> _roleManager;
	private readonly UserManager<User> _userManager;


	public UserService(UserManager<User> userManager,
					   RoleManager<Role> roleManager,
					   PermissionManager permissionManager,
					   IDbContext dataContext,
					   IDataService<User, Guid> dataService,
					   IAppSettingsService appSettingsService,
					   IOptionsSnapshot<IdentitySettings> identitySettingsOptions)
	{
		VariablesChecker.CheckIsNotNull(userManager, nameof(userManager));
		VariablesChecker.CheckIsNotNull(roleManager, nameof(roleManager));
		VariablesChecker.CheckIsNotNull(permissionManager, nameof(permissionManager));
		VariablesChecker.CheckIsNotNull(dataContext, nameof(dataContext));
		VariablesChecker.CheckIsNotNull(dataService, nameof(dataService));
		VariablesChecker.CheckIsNotNull(appSettingsService, nameof(appSettingsService));
		VariablesChecker.CheckIsNotNull(identitySettingsOptions, nameof(identitySettingsOptions));
		VariablesChecker.CheckIsNotNull(identitySettingsOptions.Value, nameof(identitySettingsOptions.Value));

		_roleManager = roleManager;
		_permissionManager = permissionManager;
		_userManager = userManager;
		_dataContext = dataContext;
		_dataService = dataService;
		_appSettingsService = appSettingsService;
		_identitySettings = identitySettingsOptions.Value;
	}


	public Task<User> Create(User user, CancellationToken ct = default) => Create(user, null, ct);

	public async Task<User> Create(User user, string? password = null, CancellationToken ct = default)
	{
		var result = string.IsNullOrEmpty(password)
			? await _userManager.CreateAsync(user)
			: await _userManager.CreateAsync(user, password);

		if (!result.Succeeded)
		{
			throw new BusinessException(string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
		}

		return await GetQueryHandler()(_dataContext.Set<User>()).SingleAsync(x => x.Id == user.Id, ct);
	}

	public Func<IQueryable<User>, IQueryable<User>> GetQueryHandler()
		=> query => query.Include(x => x.UserRoles).ThenInclude(x => x.Role)
		.Include(x => x.UserPermissions).ThenInclude(x => x.Permission);

	public async Task<User> Update(User user, CancellationToken ct = default)
	{
		var userRolesIds = user.UserRoles.Select(x => x.RoleId);
		var userPermissions = user.UserPermissions.Select(x => x.PermissionId);
		user.UserRoles = new List<UserRole>();
		user.UserPermissions = new List<UserPermission>();

		await UpdateUserRoles(user, userRolesIds, ct);
		await UpdateUserPermissions(user, userPermissions, ct);

		return await GetQueryHandler()(_dataContext.Set<User>()).SingleAsync(x => x.Id == user.Id, ct);
	}

	public async Task Validate(UserDto dto, User? entity = null, CancellationToken ct = default)
	{
		if (_identitySettings.SameUserNameEmail)
		{
			dto.UserName = dto.Email;
		}

		if (string.IsNullOrWhiteSpace(dto.UserName))
			throw new ValidationException("Логин не может быть пустым.");

		if (string.IsNullOrWhiteSpace(dto.Email))
			throw new ValidationException("Адрес электронной почты не может быть пустым.");

		dto.RootAdmin = entity == null ? false : entity.RootAdmin;

		if (dto.RootAdmin)
		{
			var superAdminRole = await _roleManager.FindByNameAsync(CoreRoles.SuperAdminRoleName);

			if (superAdminRole == null)
			{
				throw new ConflictException("Роль супер администратора не существует.");
			}

			if (entity == null || entity.UserRoles.All(x => x.RoleId != superAdminRole.Id))
			{
				throw new BusinessException("Корневого администратора нельзя лишать роли супер администратора.");
			}
		}

		if (entity != null)
		{
			foreach (var role in entity.UserRoles)
			{
				role.UserId = entity.Id;
			}

			foreach (var permission in entity.UserPermissions)
			{
				permission.UserId = entity.Id;
			}
		}
	}


	private async Task UpdateUserPermissions(User user, IEnumerable<Guid> newPermissionsIds, CancellationToken ct)
	{
		newPermissionsIds = newPermissionsIds.ToList();
		var existingUserPermissions = _dataContext.Set<UserPermission>()
			.Include(x => x.Permission)
			.Where(x => x.UserId == user.Id);

		foreach (var existingPermission in existingUserPermissions)
		{
			if (newPermissionsIds.All(x => x != existingPermission.PermissionId))
			{
				if (existingPermission.Permission is not null)
				{
					await _permissionManager.RemovePermissionFromUser(user, existingPermission.Permission, ct);
				}
			}
		}

		foreach (var newPermissionId in newPermissionsIds)
		{
			if (await existingUserPermissions.AllAsync(x => x.PermissionId != newPermissionId, ct))
			{
				var permission = await _permissionManager.FindByIdAsync(newPermissionId);

				if (permission is null)
					throw new BusinessException("Разрешение, установленное для пользователя, не существует.");

				await _permissionManager.AddPermissionForUser(user, permission);
			}
		}
	}

	private async Task UpdateUserRoles(User user, IEnumerable<Guid> newRolesIds, CancellationToken ct)
	{
		newRolesIds = newRolesIds.ToList();
		var existingUserRoles = _dataContext.Set<UserRole>()
			.AsNoTracking()
			.Include(x => x.Role)
			.Where(x => x.UserId == user.Id);

		foreach (var existingRole in existingUserRoles)
		{
			if (newRolesIds.All(x => x != existingRole.RoleId))
			{
				_dataContext.Entry(existingRole).State = EntityState.Deleted;
			}
		}

		foreach (var newRoleId in newRolesIds)
		{
			if (await existingUserRoles.AllAsync(x => x.RoleId != newRoleId, ct))
			{
				await _dataContext.Set<UserRole>().AddAsync(new UserRole { RoleId = newRoleId, UserId = user.Id }, ct);
			}
		}

		await _dataContext.SaveChangesAsync();
	}
}

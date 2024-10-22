using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Security.Claims;

using Universe.Core.Data;
using Universe.Core.Exceptions;
using Universe.Core.Membership.Model;
using Universe.Core.Utils;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Universe.Core.Membership.Authorization;

/// <summary>
/// Класс, предназначенный для управления правами.
/// </summary>
public class PermissionManager
{
	private readonly RoleManager<Role> _roleManager;
	private readonly UserManager<User> _userManager;
	private readonly IDbContext _context;


	public PermissionManager(RoleManager<Role> roleManager, UserManager<User> userManager, IDbContext context)
	{
		VariablesChecker.CheckIsNotNull(roleManager, nameof(roleManager));
		VariablesChecker.CheckIsNotNull(userManager, nameof(userManager));
		VariablesChecker.CheckIsNotNull(context, nameof(context));

		_roleManager = roleManager;
		_userManager = userManager;
		_context = context;
	}


	/// <summary>
	/// Добавляет право указанной роли.
	/// </summary>
	/// <param name="role">Роль, которой необходимо добавить право.</param>
	/// <param name="permission">Добавляемое право.</param>
	/// <exception cref="BusinessException">Возникает, если указанная роль или право имеют идентификатор по-умолчанию, что означает, что объект не сохранён.</exception>
	/// <exception cref="ConflictException">Возникает, если указанное право содержит пустое название.</exception>
	public async Task AddPermissionForRole(Role role, Permission permission, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(role, nameof(role));
		VariablesChecker.CheckIsNotNull(permission, nameof(permission));

		if (role.Id == default)
			throw new BusinessException("Невозможно добавить разрешение несохранённой роли.");

		if (permission.Id == default)
			throw new BusinessException("Невозможно добавить несохранённое разрешение.");

		if (permission.Name == null)
			throw new ConflictException("Имя разрешения не задано.");

		if (await _context.Set<RolePermission>().AnyAsync(x => x.RoleId == role.Id && x.PermissionId == permission.Id))
			throw new BusinessException("Роль уже имеет указанное разрешение.");

		await _context.Set<RolePermission>().AddAsync(new RolePermission
		{
			RoleId = role.Id,
			PermissionId = permission.Id
		}, ct);

		await _roleManager.AddClaimAsync(
			role,
			new Claim(
				CorePolicies.PermissionPolicyName,
				permission.Name,
				ClaimValueTypes.String,
				Assembly.GetExecutingAssembly().FullName));
	}

	public async Task AddPermissionForUser(User user, Permission permission, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(user, nameof(user));
		VariablesChecker.CheckIsNotNull(permission, nameof(permission));

		if (user.Id == default)
			throw new BusinessException("Невозможно добавить разрешение несохранённому пользователю.");

		if (permission.Id == default)
			throw new BusinessException("Невозможно добавить несохранённое разрешение.");

		if (permission.Name == null)
			throw new ConflictException("Имя разрешения не задано.");

		if (await _context.Set<UserPermission>().AllAsync(x => x.UserId == user.Id && x.PermissionId == permission.Id))
			throw new BusinessException("Пользователь уже имеет указанное разрешение.");

		await _context.Set<UserPermission>().AddAsync(new UserPermission
		{
			UserId = user.Id,
			PermissionId = permission.Id
		}, ct);

		await _userManager.AddClaimAsync(
			user,
			new Claim(
				CorePolicies.PermissionPolicyName,
				permission.Name,
				ClaimValueTypes.String,
				Assembly.GetExecutingAssembly().FullName));
	}

	public async Task<Permission> Create(Permission permission, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(permission, nameof(permission));

		Validate(permission);

		await _context.Set<Permission>().AddAsync(permission, ct);
		await _context.SaveChangesAsync(ct);

		return permission;
	}

	public async Task Delete(Guid permissionId, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(permissionId, nameof(permissionId));

		var existingPermission = await _context.Set<Permission>().SingleOrDefaultAsync(x => x.Id == permissionId);

		if (existingPermission is null)
			throw new ObjectNotExistsException("Удаляемое разрешение с указанным идентификатором не существует.");

		_context.Set<IdentityUserClaim<string>>().RemoveRange(
			_context.Set<IdentityUserClaim<string>>().Where(x =>
				x.ClaimType == CorePolicies.PermissionPolicyName && x.ClaimValue == existingPermission.Name));

		_context.Set<IdentityRoleClaim<string>>().RemoveRange(
			_context.Set<IdentityRoleClaim<string>>().Where(x =>
				x.ClaimType == CorePolicies.PermissionPolicyName && x.ClaimValue == existingPermission.Name));

		_context.Set<Permission>().Remove(existingPermission);

		await _context.SaveChangesAsync(ct);
	}

	public async Task<bool> Exists(Guid permissionId, CancellationToken ct = default) =>
		await _context.Set<Permission>().AnyAsync(x => x.Id == permissionId);

	public async Task<Permission?> FindByIdAsync(Guid permissionId)
		=> await _context.Set<Permission>().FindAsync(permissionId);

	public async Task<Permission?> FindByNameAsync(string permissionName, CancellationToken ct = default)
		=> await _context.Set<Permission>().SingleOrDefaultAsync(x => EF.Functions.Like(x.Name!, permissionName));

	public async Task<IEnumerable<Permission?>> GetAllPermissions(CancellationToken ct = default) =>
		await _context.Set<Permission>().ToListAsync(ct);

	public async Task<IEnumerable<Permission?>> GetRolePermissions(Role role, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(role, nameof(role));

		if (role.Id == default)
			throw new BusinessException("Невозможно запросить разрешения для несохранённой роли.");

		return await _context.Set<RolePermission>()
			.Include(x => x.Role)
			.Include(x => x.Permission)
			.Where(x => x.RoleId == role.Id)
			.Select(x => x.Permission)
			.ToListAsync(ct);
	}

	public async Task<IEnumerable<Permission?>> GetUserPermissions(User user, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(user, nameof(user));

		if (user.Id == default)
			throw new BusinessException("Невозможно запросить разрешения для несохранённого пользователя.");

		return await _context.Set<UserPermission>()
			.Include(x => x.User)
			.Include(x => x.Permission)
			.Where(x => x.UserId == user.Id)
			.Select(x => x.Permission)
			.ToListAsync(ct);
	}

	public async Task<bool> HasPermission(User user, string permission, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(user, nameof(user));
		VariablesChecker.CheckIsNotNullOrEmpty(permission, nameof(permission));

		if (user.Id == default)
			throw new BusinessException("У несохранённого пользователя не может быть прав.");

		var userRolesIds = await _context.Set<UserRole>()
			.Where(x => x.UserId == user.Id)
			.Select(x => x.RoleId)
			.Distinct()
			.ToListAsync(ct);

		return 
			await _context.Set<UserPermission>()
				.Include(x => x.Permission)
				.AnyAsync(x => x.UserId == user.Id && x.Permission != null && x.Permission.Name == permission, ct) ||
			await _context.Set<RolePermission>()
				.Include(x => x.Permission)
				.AnyAsync(x => userRolesIds.Contains(x.RoleId) && x.Permission != null && x.Permission.Name == permission, ct);
	}

	public async Task<bool> HasPermission(Role role, string permission, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(role, nameof(role));
		VariablesChecker.CheckIsNotNullOrEmpty(permission, nameof(permission));

		if (role.Id == default)
			throw new BusinessException("У несохранённой роли не может быть прав.");

		return await _context.Set<RolePermission>()
			.Include(x => x.Permission)
			.AnyAsync(x => x.RoleId == role.Id && x.Permission != null && x.Permission.Name == permission, ct);
	}

	public bool HasPermission(ClaimsPrincipal user, string permission)
	{
		VariablesChecker.CheckIsNotNull(user, nameof(user));
		VariablesChecker.CheckIsNotNullOrEmpty(permission, nameof(permission));

		return user.Identity?.IsAuthenticated == true && user.HasClaim(CorePolicies.PermissionPolicyName, permission);
	}

	public async Task RemovePermissionFromRole(Role role, Permission permission, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(role, nameof(role));
		VariablesChecker.CheckIsNotNull(permission, nameof(permission));

		if (role.Id == default)
			throw new BusinessException("Невозможно удалить разрешение у несохранённой роли.");

		if (permission.Id == default)
			throw new BusinessException("Невозможно удалить несохранённое разрешение.");

		if (permission.Name == null)
			throw new ConflictException("Имя разрешения не задано.");

		var rolePermission = await _context.Set<RolePermission>()
			.SingleOrDefaultAsync(x => x.RoleId == role.Id && x.PermissionId == permission.Id, ct);

		if (rolePermission is not null)
			_context.Set<RolePermission>().Remove(rolePermission);

		var claim = (await _roleManager.GetClaimsAsync(role))
				.Single(x => x.Type == CorePolicies.PermissionPolicyName && x.Value == permission.Name);

		if (claim is null)
			await _context.SaveChangesAsync(ct);
		else
			await _roleManager.RemoveClaimAsync(role, claim);
	}

	public async Task RemovePermissionFromUser(User user, Permission permission, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(user, nameof(user));
		VariablesChecker.CheckIsNotNull(permission, nameof(permission));

		if (user.Id == default)
			throw new BusinessException("Невозможно удалить разрешение у несохранённого пользователя.");

		if (permission.Id == default)
			throw new BusinessException("Невозможно удалить несохранённое разрешение.");

		if (permission.Name == null)
			throw new ConflictException("Имя разрешения не задано.");

		var userPermission = await _context.Set<UserPermission>()
			.SingleOrDefaultAsync(x => x.UserId == user.Id && x.PermissionId == permission.Id, ct);
		if (userPermission is not null)
			_context.Set<UserPermission>().Remove(userPermission);

		await _userManager.RemoveClaimsAsync(
			user,
			(await _userManager.GetClaimsAsync(user))
				.Where(x => x.Type == CorePolicies.PermissionPolicyName && x.Value == permission.Name));
	}

	public async Task<Permission> Update(Permission permission, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(permission, nameof(permission));

		var existingPermission = await _context.Set<Permission>().SingleOrDefaultAsync(x => x.Id == permission.Id);

		if (existingPermission is null)
			throw new ObjectNotExistsException("Обновляемое разрешение с указанным идентификатором не существует.");

		Validate(permission);

		existingPermission.Name = permission.Name;

		var usersPermissionClaims = await _context.Set<IdentityUserClaim<string>>()
			.Where(x => x.ClaimType == CorePolicies.PermissionPolicyName && x.ClaimValue == existingPermission.Name)
			.ToListAsync(ct);

		foreach (var userClaim in usersPermissionClaims)
			userClaim.ClaimValue = permission.Name;

		var rolesPermissionClaims = await _context.Set<IdentityRoleClaim<string>>()
			.Where(x => x.ClaimType == CorePolicies.PermissionPolicyName && x.ClaimValue == existingPermission.Name)
			.ToListAsync(ct);

		foreach (var roleClaim in rolesPermissionClaims)
			roleClaim.ClaimValue = permission.Name;

		await _context.SaveChangesAsync(ct);

		return permission;
	}


	private void Validate(Permission? permission)
	{
		if (permission is not null)
		{
			if (string.IsNullOrWhiteSpace(permission.Name))
				throw new ValidationException("Имя разрешения не задано.");

			if (permission.Id == default)
			{
				if (_context.Set<Permission>().Any(x => x.Name == permission.Name))
					throw new ValidationException($"Резрешение с именем {permission.Name} уже существует.");
			}
			else
			{
				if (_context.Set<Permission>().Any(x => x.Name == permission.Name && x.Id != permission.Id))
					throw new ValidationException($"Разрешение с именем {permission.Name} уже существует.");
			}
		}
	}
}

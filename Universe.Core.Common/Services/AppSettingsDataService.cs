using System.ComponentModel.DataAnnotations;
using System.Text.Json;

using Universe.Core.Data;
using Universe.Core.Exceptions;
using Universe.Core.Extensions;
using Universe.Core.JsonSerialization;
using Universe.Core.Membership.Model;
using Universe.Core.ModuleBinding;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Universe.Core.AppConfiguration.Services;
using Universe.Core.Common.Model;
using Universe.Core.Membership;

namespace Universe.Core.Common.Services;

/// <inheritdoc cref="IAppSettingsSetService"/>
public class AppSettingsDataService : IAppSettingsDataService
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IServiceProvider _serviceProvider;
	private readonly IDbContext _context;
	private readonly IAppSettingsService _appSettingsService;


	public AppSettingsDataService(
		IHttpContextAccessor httpContextAccessor,
		IServiceProvider serviceProvider,
		IDbContext context,
		IAppSettingsService appSettingsService)
	{
		_httpContextAccessor = httpContextAccessor;
		_serviceProvider = serviceProvider;
		_context = context;
		_appSettingsService = appSettingsService;
	}


	public async Task<IEnumerable<AppSettingsSet>> GetAllowedForCurrentUserSettings(CancellationToken ct = default)
	{
		if (_httpContextAccessor.HttpContext is null)
			throw new InvalidOperationException("Не задан контекст HTTP запроса.");

		Guid.TryParse(_httpContextAccessor.HttpContext?.GetUserId(), out Guid currentUserId);
		var currentUser = default
			? null
			: await _context.Set<User>()
				.Include(x => x.UserRoles).ThenInclude(x => x.Role)
				.Include(x => x.UserPermissions)
				.SingleOrDefaultAsync(x => x.Id == currentUserId, ct);

		var queriable = _context.Set<AppSettingsSet>()
			.Include(x => x.Roles)
			.Include(x => x.Permissions).ThenInclude(x => x.Permission);

		if (currentUser is null)
			return queriable.Where(x => x.AggregatedRole == AggregatedRole.Anyone);

		// Супер админу доступны все настройки
		if (currentUser.UserRoles.Any(x => x.Role?.Name == CoreRoles.SuperAdminRoleName))
			return await queriable.ToListAsync(ct);

		var currentUserRolesIds = currentUser.UserRoles.Select(x => x.RoleId);
		var currentUserPermissionIds = currentUser.UserPermissions.Select(x => x.PermissionId)
			.Union(_context.Set<RolePermission>().Where(x => currentUserRolesIds.Contains(x.RoleId)).Select(x => x.PermissionId)).ToList();

		return (await queriable.ToListAsync(ct)).Where(x =>
			x.AggregatedRole == AggregatedRole.Authenticated ||
			x.Roles.Select(y => y.RoleId).Intersect(currentUserRolesIds).Any() ||
			GetAppSettingsSetPermissionsIds(x).Intersect(currentUserPermissionIds).Any());
	}

	public async Task<AppSettingsSet> SaveSettings(AppSettingsSet settings, CancellationToken ct = default)
	{
		if (_httpContextAccessor.HttpContext is null)
			throw new InvalidOperationException("Не задан контекст HTTP запроса.");

		if (_httpContextAccessor.HttpContext.User.Identity is null ||
			!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated ||
			!Guid.TryParse(_httpContextAccessor.HttpContext.GetUserId(), out Guid currentUserId))
		{
			throw new ForbiddenException("Нет прав на изменение набора настроек.");
		}

		var currentUser = await _context.Set<User>()
			.Include(x => x.UserRoles).ThenInclude(x => x.Role)
			.Include(x => x.UserPermissions)
			.SingleOrDefaultAsync(x => x.Id == currentUserId, ct);

		if (currentUser is null)
			throw new ConflictException("Пользователь, соответствующий текущей личности, не найден.");

		if (string.IsNullOrEmpty(settings.Value))
			throw new ValidationException($"Сохраняемый набор настроек '{settings.Name}' содержит пустое значение.");

		var configType = AssembliesManager.GetProjectAssemblies()
			.SelectMany(x => x.GetTypes())
			.SingleOrDefault(x => x.Name == settings.Name);

		if (configType is null)
			throw new ObjectNotExistsException($"Тип данных набора настроек '{settings.Name}' не существует.");

		object? settingsSetValue;
		try
		{
			settingsSetValue = JsonSerializer.Deserialize(settings.Value, configType, JsonSerializerOptionsProvider.GetOptions());

			if (settingsSetValue is null)
				throw new Exception("Сохранённый набор настроек стал пустым объектом.");
		}
		catch (Exception ex)
		{
			throw new ValidationException($"Сохраняемый набор настроек '{settings.Name}' содержит неверный формат данных: {ex.Message}");
		}

		var existingSettings = await _context.Set<AppSettingsSet>()
			.AsNoTracking()
			.SingleOrDefaultAsync(x => x.Name == settings.Name, ct);

		if (existingSettings is null)
			throw new ObjectNotExistsException($"Наюор настроек '{settings.Name}' не существует в хранилище данных.");
		
		if (currentUser.UserRoles.All(x => x.Role?.Name != CoreRoles.SuperAdminRoleName)) // Пропустить проверку прав для SuperAdmin-а
		{
			var userRolesIds = currentUser.UserRoles.Select(x => x.RoleId);
			var userPermissionsIds = currentUser.UserPermissions.Select(x => x.PermissionId)
				.Union(_context.Set<RolePermission>().Where(x => userRolesIds.Contains(x.RoleId)).Select(x => x.PermissionId))
				.Distinct();

			if (
				await _context.Set<AppSettingsSetRole>()
					.AllAsync(x => x.AppSettingsSetId != existingSettings.Id || !userRolesIds.Contains(x.RoleId) || !x.AllowChange) &&
				await _context.Set<AppSettingsSetPermission>()
					.AllAsync(x => x.AppSettingsSetId != existingSettings.Id || !userPermissionsIds.Contains(x.PermissionId) || !x.AllowChange))
			{
				throw new ForbiddenException("Нет прав на изменение набора настроек.");
			}
		}

		settings.Id = existingSettings.Id;
		_context.Entry(settings).State = EntityState.Modified;
		await _context.SaveChangesAsync(ct);

		await _appSettingsService.ReplaceSettings(configType, settingsSetValue, _serviceProvider);

		return settings;
	}


	private List<Guid> GetAppSettingsSetPermissionsIds(AppSettingsSet set)
	{
		var setRolesIds = set.Roles.Select(x => x.RoleId).ToList();
		return set.Permissions.Select(x => x.PermissionId)
			.Union(_context.Set<RolePermission>()
				.Where(x => setRolesIds.Contains(x.RoleId))
				.Select(y => y.PermissionId))
			.ToList();
	}
}
using Universe.Core.Common.Model;

namespace Universe.Core.Common.Services;

public interface IAppSettingsDataService
{
	Task<IEnumerable<AppSettingsSet>> GetAllowedForCurrentUserSettings(CancellationToken ct = default);

	Task<AppSettingsSet> SaveSettings(AppSettingsSet settings, CancellationToken ct = default);
}

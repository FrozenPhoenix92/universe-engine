using Universe.Core.AppConfiguration.Exceptions;
using Universe.Core.ModuleBinding.Bindings;
using Universe.Files.Configuration;
using Universe.Files.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Universe.Files.ModuleBinding;

public class DependenciesModuleBinding : IDependenciesModuleBinding
{
    public void AddDependencies(IServiceCollection services, IConfiguration configuration)
    {
        var fileStorageSettingsSection = configuration.GetSection(nameof(FileStorageSettings));
        var fileStorageSettings = fileStorageSettingsSection?.Get<FileStorageSettings>();
        if (fileStorageSettingsSection is null || fileStorageSettings is null)
            throw new MissedConfigurationSectionException(nameof(FileStorageSettings));
        if (string.IsNullOrWhiteSpace(fileStorageSettings.Folder))
            throw new MissedConfigurationSectionException(nameof(FileStorageSettings.Folder));
        services.Configure<FileStorageSettings>(fileStorageSettingsSection);

        switch (fileStorageSettings.StorageType)
        {
            case FileStorageType.LocalFolder:
                services.AddScoped<IFileStorageService, LocalFolderFileStorageService>();
                break;
            default:
                throw new InvalidConfigurationSectionException(nameof(fileStorageSettings.StorageType));
        }
    }
}

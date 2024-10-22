using Universe.Core.Data;

namespace Universe.Core.AppConfiguration.Model;

/// <summary>
/// Представляет набор найстроек конфигурации системы.
/// </summary>
public class AppSettingsSetCore : IEntity
{
    /// <inheritdoc/>
    public int Id { get; set; }

    /// <summary>
    /// Название набора.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Значение в виде строки JSON.
    /// </summary>
    public string? Value { get; set; }
}

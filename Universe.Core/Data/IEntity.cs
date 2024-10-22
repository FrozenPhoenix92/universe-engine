namespace Universe.Core.Data;

/// <summary>
/// Базовый интерфейс для сущностей, содержащих первиячный ключ.
/// </summary>
/// <typeparam name="TKey">Тип первичного ключа.</typeparam>
public interface IEntity<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Индентификатор сущности.
    /// </summary>
    TKey Id { get; set; }
}

/// <summary>
/// Базовый интерфейс для сущностей, содержащих целочисленный первиячный ключ.
/// </summary>
public interface IEntity : IEntity<int>
{
}

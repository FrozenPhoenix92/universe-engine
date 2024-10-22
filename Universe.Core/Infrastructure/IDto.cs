namespace Universe.Core.Infrastructure
{
    /// <summary>
    /// Базовый интерфейс для DTO, соответствующих какому либо классу сущностей, содержащих первиячный ключ.
    /// </summary>
    /// <typeparam name="TKey">Тип первичного ключа.</typeparam>
    public interface IDto<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
    }

    /// <summary>
    /// Базовый интерфейс для DTO, соответствующих какому либо классу сущностей, содержащих целочисленный первиячный ключ.
    /// </summary>
    public interface IDto : IDto<int>
    {
    }
}

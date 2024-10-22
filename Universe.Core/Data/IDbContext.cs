using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Universe.Core.Data;

/// <summary>
/// Представляет базовый набор функционала для контекста данных.
/// </summary>
public interface IDbContext
{
    /// <summary>
    /// Предоставляет доступ к информации и операциям, связанным с базой данных, для этого контекста.
    /// </summary>
    DatabaseFacade Database { get; }

    /// <summary>
    /// Создаёт объект <see cref="DbSet{T}" /> для управления сущностями типа <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    DbSet<T> Set<T>() where T : class;

    /// <summary>
    /// Предоставляет доступ к отслеживанию изменений для сущности <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <param name="item">Экземпляр сущности.</param>
    EntityEntry<TEntity> Entry<TEntity>(TEntity item) where TEntity : class;

    /// <summary>
    /// Синхронное сохранение изменений.
    /// </summary>
    /// <returns>Количество записей состояния, записанных в базу данных.</returns>
    int SaveChanges();

    /// <summary>
    /// Асинхронное сохранение изменений.
    /// </summary>
    /// <returns>Количество записей состояния, записанных в базу данных.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

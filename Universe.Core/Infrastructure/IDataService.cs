using Universe.Core.Data;
using Universe.Core.QueryHandling;

using System.Linq.Expressions;

namespace Universe.Core.Infrastructure;

/// <summary>
/// Определяет базовый набор наиболее часто используемых типичных операций, таких, как CRUD или фильтрация, над множеством сущностей.
/// </summary>
/// <typeparam name="TContext">Тип контекста данных.</typeparam>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
/// <typeparam name="TKey">Тип идентификатора сущности.</typeparam>
public partial interface IDataService<TEntity, TKey, TContext>
	where TEntity : class, IEntity<TKey>
	where TKey : IEquatable<TKey>
	where TContext : IDbContext
{
    /// <summary>
    /// Контекст данных.
    /// </summary>
    TContext Context { get; }


    /// <summary>
    /// Выполняет операцию создания новой сущности.
    /// </summary>
    /// <param name="entity">Создаваемая сущность.</param>
    /// <returns>Созданная сущность.</returns>
    Task<TEntity> Create(TEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Выполняет операцию создания новой сущности,
    /// предварительно применив пользовательскую операцию непосредственно перед сохранением изменений.
    /// </summary>
    /// <param name="entity">Создаваемая сущность.</param>
    /// <param name="beforeSave">Операция, выполняемая непосредственно перед сохранением изменений.</param>
    /// <returns>Созданная сущность.</returns>
    Task<TEntity> Create(TEntity entity, Func<TEntity, TContext, CancellationToken, Task>? beforeSave, CancellationToken ct = default);

    /// <summary>
    /// Выполняет операцию удаления существующей сущности с указанным идентификатором.
    /// </summary>
    /// <param name="id">Идентификатор удаляемой сущности.</param>
    Task Delete(TKey id, CancellationToken ct = default);

    /// <summary>
    /// Выполняет операцию удаления существующей сущности с указанным идентификатором,
    /// предварительно применив пользовательскую операцию непосредственно перед сохранением изменений.
    /// </summary>
    /// <remarks>
    /// Подобная реализация может пригодиться для предварительного удаления связанных объектов
    /// во избежание конфликтов с ограничениями внешнего ключа.
    /// </remarks>
    /// <param name="id">Идентификатор удаляемой сущности.</param>
    /// <param name="beforeSave">Операция, выполняемая непосредственно перед сохранением изменений.</param>
    Task Delete(TKey id, Func<TEntity, TContext, CancellationToken, Task>? beforeSave, CancellationToken ct = default);

    /// <summary>
    /// Выполняет операцию удаления всего множества сущностей.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    Task DeleteAll(CancellationToken ct = default);

    /// <summary>
    /// Выполняет операцию удаления всего множества сущностей.
    /// </summary>
    /// <param name="queryHandler">Польовательская операция над объектом запроса, определяющая множество удаляемых сущностей.</param>
    Task DeleteAll(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryHandler, CancellationToken ct = default);

	/// <summary>
	/// Определяет существование сущности с указанным идентификатором.
	/// </summary>
	/// <param name="id">Искомый идентификатор сущности.</param>
	Task<bool> Exists(TKey id, CancellationToken ct = default);

	/// <summary>
	/// Определяет существование сущности, применяя указанную функцию соответствия.
	/// </summary>
	/// <param name="expression">Выражение, определяющае условие соответствия объекта данных необходимым критериям.</param>
	Task<bool> Exists(Expression<Func<TEntity, bool>> expression, CancellationToken ct = default);

	/// <summary>
	/// Выполняет запрос сущности с указанном идентификатором.
	/// </summary>
	/// <param name="id">Идентификатор запрашиваемой сущности.</param>
	/// <returns>Сущность с указанным идентификатором.</returns>
	Task<TEntity?> Get(TKey id, CancellationToken ct = default);

    /// <summary>
    /// Выполняет запрос сущности с указанном идентификатором.
    /// </summary>
    /// <param name="id">Идентификатор запрашиваемой сущности.</param>
    /// <param name="queryHandler">Пользовательская операция, предварительно приняемая к объекту запроса.</param>
    /// <returns>Сущность с указанным идентификатором.</returns>
    Task<TEntity?> Get(TKey id, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryHandler, CancellationToken ct = default);

    /// <summary>
    /// Выполняет запрос сущности, применяя указанное пользовательское выражение.
    /// </summary>
    /// <param name="queryHandler">Пользовательское выражение, возвращающее запрашиваемую сущность.</param>
    /// <returns>Сущность соответствующая указанному пользовательскому выражению.</returns>
    Task<TEntity?> Get(Expression<Func<TEntity, bool>> expression, CancellationToken ct = default);

    /// <summary>
    /// Выполняет запрос множества всех сущностей.
    /// </summary>
    /// <returns>Множество всех сущностей./returns>
    Task<IEnumerable<TEntity>> GetAll(CancellationToken ct = default);

    /// <summary>
    /// Выполняет запрос множества сущностей, применяя команду с данными для настройки запроса.
    /// </summary>
    /// <param name="command">Команда, определяющая параметры объекта запроса.</param>
    /// <returns>Множество соответствующих команде сущностей./returns>
    Task<IEnumerable<TEntity>> GetAll(QueryCommand? command, CancellationToken ct = default);

    /// <summary>
    /// Выполняет запрос множества сущностей, применяя указанное пользовательское выражение.
    /// </summary>
    /// <param name="queryHandler">Пользовательское выражение, применяемое к запросу.</param>
    /// <returns>Множество сущностей, соответствующих указанному пользовательскому выражению.</returns>
    Task<IEnumerable<TEntity>> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryHandler, CancellationToken ct = default);

    /// <summary>
    /// Выполняет запрос множества сущностей, применяя команду с данными для настройки запроса и указанное пользовательское выражение.
    /// </summary>
    /// <param name="command">Команда, определяющая параметры объекта запроса.</param>
    /// <param name="queryHandler">Пользовательское выражение, применяемое к запросу.</param>
    /// <returns>Множество сущностей, соответствующих команде и указанному пользовательскому выражению.</returns>
    Task<IEnumerable<TEntity>> GetAll(QueryCommand? command, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryHandler, CancellationToken ct = default);

	/// <summary>
	/// Выполняет запрос колчичества всех сущностей.
	/// </summary>
	/// <returns>Множество всех сущностей./returns>
	Task<int> GetTotal(CancellationToken ct = default);

	/// <summary>
	/// Выполняет запрос колчичества сущностей, применяя команду с данными для настройки запроса.
	/// </summary>
	/// <param name="command">Команда, определяющая параметры объекта запроса.</param>
	/// <returns>Множество соответствующих команде сущностей./returns>
	Task<int> GetTotal(QueryCommand? command, CancellationToken ct = default);

	/// <summary>
	/// Выполняет запрос колчичества сущностей, применяя указанное пользовательское выражение.
	/// </summary>
	/// <param name="queryHandler">Пользовательское выражение, применяемое к запросу.</param>
	/// <returns>Множество сущностей, соответствующих указанному пользовательскому выражению.</returns>
	Task<int> GetTotal(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryHandler, CancellationToken ct = default);

	/// <summary>
	/// Выполняет запрос колчичества сущностей, применяя команду с данными для настройки запроса и указанное пользовательское выражение.
	/// </summary>
	/// <param name="command">Команда, определяющая параметры объекта запроса.</param>
	/// <param name="queryHandler">Пользовательское выражение, применяемое к запросу.</param>
	/// <returns>Множество сущностей, соответствующих команде и указанному пользовательскому выражению.</returns>
	Task<int> GetTotal(QueryCommand? command, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryHandler, CancellationToken ct = default);

	/// <summary>
	/// Выполняет операцию обновления существующей сущности.
	/// </summary>
	/// <param name="entity">Обновляемая сущность.</param>
	/// <returns>Обновлённая сущность.</returns>
	Task<TEntity> Update(TEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Выполняет операцию обновления существующей сущности,
    /// предварительно применив пользовательскую операцию непосредственно перед сохранением изменений.
    /// </summary>
    /// <param name="entity">Обновляемая сущность.</param>
    /// <param name="beforeSave">Операция, выполняемая непосредственно перед сохранением изменений.</param>
    /// <returns>Обновлённая сущность.</returns>
    Task<TEntity> Update(TEntity entity, Func<TEntity, TContext, CancellationToken, Task>? beforeSave, CancellationToken ct = default);
}

/// <inheritdoc/>
public interface IDataService<TEntity, TKey> : IDataService<TEntity, TKey, IDbContext>
	where TEntity : class, IEntity<TKey>
	where TKey : IEquatable<TKey>
{
}

/// <inheritdoc/>
public interface IDataService<TEntity> : IDataService<TEntity, int>
	where TEntity : class, IEntity
{
}

using Universe.Core.Data;
using Universe.Core.QueryHandling;

namespace Universe.Core.Infrastructure;

/// <summary>
/// Базовый интерфейс для остальных интерфейсов, определяющих пользовательское переопределение реализации по умолчанию операции над множеством сущностей.
/// </summary>
public interface IDataOperation
{
}

#region Интерфейсы для переопределения базовой реализации часто используемых над множестом данных операций

/// <summary>
/// Представляет пользовательское переопределение операции создания новой сущности.
/// </summary>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
public interface ICreateDataOperation<TEntity, TKey> : IDataOperation
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Выполняет пользовательское переопределение операции создания новой сущности.
    /// </summary>
    /// <param name="entity">Сущность, которую необходимо создать.</param>
    /// <returns>Созданная сущность.</returns>
    Task<TEntity> Create(TEntity entity, CancellationToken ct = default);
}

/// <summary>
/// Представляет пользовательское переопределение операции создания новой сущности.
/// </summary>
public interface ICreateDataOperation<TEntity> : ICreateDataOperation<TEntity, int>
    where TEntity : class, IEntity
{
}

/// <summary>
/// Представляет пользовательское переопределение операции удаления существующей сущности по указанному идентификатору.
/// </summary>
/// <typeparam name="TKey">Тип идентификатора сущности.</typeparam>
public interface IDeleteDataOperation<TKey> : IDataOperation
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Выполняет пользовательское переопределение операции удаления существующей сущности по указанному идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор запрашиваемой сущности.</param>
    Task Delete(TKey id, CancellationToken ct = default);
}

/// <summary>
/// Представляет пользовательское переопределение операции удаления существующей сущности по указанному идентификатору.
/// </summary>
public interface IDeleteDataOperation : IDeleteDataOperation<int>
{
}

/// <summary>
/// Представляет пользовательское переопределение операции удаления всего множества сущностей.
/// </summary>
public interface IDeleteAllDataOperation : IDataOperation
{
	/// <summary>
	/// Выполняет пользовательское переопределение операции удаления всего множества сущностей.
	/// </summary>
	Task DeleteAll(CancellationToken ct = default);
}

/// <summary>
/// Представляет пользовательское переопределение операции запроса сущности по указанному идентификатору.
/// </summary>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
/// <typeparam name="TKey">Тип идентификатора сущности.</typeparam>
public interface IGetDataOperation<TEntity, TKey> : IDataOperation
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Выполняет пользовательское переопределение операции запроса сущности по указанному идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор запрашиваемой сущности.</param>
    /// <returns>Сущность с указанным идентификатором.</returns>
    Task<TEntity?> Get(TKey id, CancellationToken ct = default);
}

/// <summary>
/// Представляет пользовательское переопределение операции запроса сущности по указанному идентификатору.
/// </summary>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
public interface IGetDataOperation<TEntity> : IGetDataOperation<TEntity, int>
    where TEntity : class, IEntity
{
}

/// <summary>
/// Представляет пользовательское переопределение операции запроса множества сущностей.
/// </summary>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
/// <typeparam name="TKey">Тип идентификатора сущности.</typeparam>
public interface IGetAllDataOperation<TEntity, TKey> : IDataOperation
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Выполняет пользовательское переопределение операции запроса множества сущностей.
    /// </summary>
    /// <param name="command">Команда, определяющая параметры объекта запроса.</param>
    /// <returns>Множество соответствующих команде сущностей.</returns>
    Task<IEnumerable<TEntity>> GetAll(QueryCommand? command, CancellationToken ct = default);
}

/// <summary>
/// Представляет пользовательское переопределение операции запроса множества сущностей.
/// </summary>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
public interface IGetAllDataOperation<TEntity> : IGetAllDataOperation<TEntity, int>
    where TEntity : class, IEntity
{
}

/// <summary>
/// Представляет пользовательское переопределение операции запроса количества сущностей.
/// </summary>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
/// <typeparam name="TKey">Тип идентификатора сущности.</typeparam>
public interface IGetTotalDataOperation<TEntity, TKey> : IDataOperation
	where TEntity : class, IEntity<TKey>
	where TKey : IEquatable<TKey>
{
	/// <summary>
	/// Выполняет пользовательское переопределение операции запроса множества сущностей.
	/// </summary>
	/// <param name="command">Команда, определяющая параметры объекта запроса.</param>
	/// <returns>Множество соответствующих команде сущностей.</returns>
	Task<int> GetTotal(QueryCommand? command, CancellationToken ct = default);
}

/// <summary>
/// Представляет пользовательское переопределение операции запроса количества сущностей.
/// </summary>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
public interface IGetTotalDataOperation<TEntity> : IGetTotalDataOperation<TEntity, int>
	where TEntity : class, IEntity
{
}

/// <summary>
/// Представляет пользовательское переопределение операции обновления существующей сущности.
/// </summary>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
/// <typeparam name="TKey">Тип идентификатора сущности.</typeparam>
public interface IUpdateDataOperation<TEntity, TKey> : IDataOperation
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Выполняет пользовательское переопределение операции обновления существующей сущности.
    /// </summary>
    /// <param name="entity">Обновляемая сущность.</param>
    /// <returns>Обновлённая сущность.</returns>
    Task<TEntity> Update(TEntity entity, CancellationToken ct = default);
}

/// <summary>
/// Представляет пользовательское переопределение операции обновления существующей сущности.
/// </summary>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
public interface IUpdateDataOperation<TEntity> : IUpdateDataOperation<TEntity, int>
    where TEntity : class, IEntity
{
}

#endregion

#region Aggregated CRUD interfaces

/// <summary>
/// Представляет список пользовательских переопределений операций CRUD: создание новой сущности, обновление существующей сущности,
/// удаление существующей сущности, запрос одной сущности по идентификатору и запрос множества сущностей с командой настроек объекта запроса.
/// </summary>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
/// <typeparam name="TKey">Тип идентификатора.</typeparam>
public interface ICrudDataOperations<TEntity, TKey> :
    ICreateDataOperation<TEntity, TKey>,
    IDeleteDataOperation<TKey>,
    IGetDataOperation<TEntity, TKey>,
    IGetAllDataOperation<TEntity, TKey>,
    IUpdateDataOperation<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
}

/// <summary>
/// Представляет список пользовательских переопределений операций CRUD: создание новой сущности, обновление существующей сущности,
/// удаление существующей сущности, чтение одной сущности с указанным идентификатором и чтение множества сущностей, соответствующих
/// команде настроек запроса к хранилищу данных.
/// </summary>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
public interface ICrudDataOperations<TEntity> : ICrudDataOperations<TEntity, int>
    where TEntity : class, IEntity
{
}

#endregion

/// <summary>
/// Представляет пользовательское преобразование запроса к хранилищу данных, используемого во время операций чтения.
/// одной или множества сущностей.
/// </summary>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
public interface IChangeQueryDataOperation<TEntity> : IDataOperation where TEntity : class
{
    /// <summary>
    /// Возвращает функцию пользовательского преобразования запроса к хранилищу данных.
    /// </summary>
    Func<IQueryable<TEntity>, IQueryable<TEntity>> GetQueryHandler();
}

/// <summary>
/// Представляет метод валидации DTO, используемого в операцях создания и обновления сущности.
/// </summary>
/// <typeparam name="TDto">Тип DTO.</typeparam>
/// <typeparam name="TEntity">Тип сущности в случае операции обновления.</typeparam>
/// <typeparam name="TKey">Тип идентификатора DTO.</typeparam>
public interface IValidateDataOperation<TDto, TEntity, TKey> : IDataOperation
    where TKey : IEquatable<TKey>
	where TEntity : class, IEntity<TKey>
	where TDto : class, IDto<TKey>
{
    /// <summary>
    /// Проверяет корректность DTO, используемого в операцях создания и обновления сущности.
    /// </summary>
    /// <param name="dto">Проверяемый DTO.</param>
    Task Validate(TDto dto, TEntity? entity = null, CancellationToken ct = default);
}

/// <summary>
/// Представляет метод валидации DTO, используемого в операцях создания и обновления сущности.
/// </summary>
/// <typeparam name="TDto">Тип DTO.</typeparam>
public interface IValidateDataOperation<TDto, TEntity> : IValidateDataOperation<TDto, TEntity, int>
	where TEntity : class, IEntity
	where TDto : class, IDto
{
}

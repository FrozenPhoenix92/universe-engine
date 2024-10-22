using Universe.Core.Data;
using Universe.Core.Utils;
using Universe.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Universe.Core.QueryHandling;

namespace Universe.Core.Infrastructure;

/// <summary>
/// Базовый класс API контроллера, реализующий наиболее часто используемые операции управления
/// множествами данных, такие, как CRUD или фильтрация.
/// </summary>
public abstract class DataControllerCore<TEntity, TDto, TKey, TContext> : ControllerCore
	where TEntity : class, IEntity<TKey>
	where TDto : class, IDto<TKey>
	where TKey : IEquatable<TKey>
	where TContext : IDbContext
{
	/// <value>
	/// Сервис со стандартной универсальной реализацией операций управления множествами данных.
	/// </value>
	protected IDataService<TEntity, TKey, TContext> DefaultDataOperationsService { get; }

	/// <value>
	/// Сервис, переопределяющий стандартную реализацию операций управления над множествами данных,
	/// и содержащий дополнительные операции для данного конкретного случая.
	/// </value>
	private IDataOperation? CustomDataOperationsService { get; }

	/// <summary>
	/// Создаёт новый экземпляр контроллера с реализацией операций управления множествами данных.
	/// По умаолчанию все операции выполняет сервис со стандартной реализацией, если в переопределяющем сервисе не реализована
	/// аналогичная операция.
	/// </summary>
	/// <param name="dataService">Сервис со стандартной реализацией операций</param>
	/// <param name="dataHandler">Сервис, переопределяющий стандартную реализацию операций над множествами
	/// и содержащий дополнительные операции для данного конкретного случая.</param>
	protected DataControllerCore(
		IMapper mapper,
		IDataService<TEntity, TKey, TContext> defaultDataOperationsService,
		IDataOperation? customDataOperationsService = null)
		: base(mapper)
	{
		VariablesChecker.CheckIsNotNull(mapper, nameof(mapper));
		VariablesChecker.CheckIsNotNull(defaultDataOperationsService, nameof(defaultDataOperationsService));

		DefaultDataOperationsService = defaultDataOperationsService;
		CustomDataOperationsService = customDataOperationsService;
	}

	/// <summary>
	/// Создаёт новый экземпляр контроллера с реализацией операций управления множествами данных.
	/// Все операции должны быть реализованы в переопределяющем сервисе.
	/// </summary>
	/// <param name="dataHandler">Сервис, переопределяющий стандартную реализацию операций над множествами
	/// и содержащий дополнительные операции для данного конкретного случая.</param>
	protected DataControllerCore(IMapper mapper, IDataOperation customDataOperationsService) : base(mapper)
	{
		VariablesChecker.CheckIsNotNull(mapper, nameof(mapper));
		VariablesChecker.CheckIsNotNull(customDataOperationsService, nameof(customDataOperationsService));

		CustomDataOperationsService = customDataOperationsService;
	}


	protected virtual Func<TEntity, TContext, CancellationToken, Task>? BeforeCreateOperation { get; } = null;
	protected virtual Func<TEntity, TContext, CancellationToken, Task>? BeforeDeleteOperation { get; } = null;

	/// <summary>
	/// Функция, позволяющая задать дополнительную логику перед сохранением операции удаления всей коллекции элементов.
	/// По умолчанию не задана.
	/// </summary>
	protected virtual Func<TContext, CancellationToken, Task>? BeforeDeleteAllOperation { get; } = null;

	/// <summary>
	/// Функция, позволяющая задать дополнительную логику перед сохранением операции обновления существующего элемента.
	/// По умолчанию не задана.
	/// </summary>
	protected virtual Func<TEntity, TContext, CancellationToken, Task>? BeforeUpdateOperation { get; } = null;

	/// <summary>
	/// Свойство, блокирующее действие операции удаления всего множества элементов. Изначально возвращает 'false' и
	/// тем самым делает операцию недоступной в связи с её высокой степенью ответственности и большщим объёмом воздействия.
	/// Для разрешения данной операции необходимо в наследуемом классе контроллера переопределить значение этого свойства.
	/// </summary>
	protected virtual bool DeleteAllOperationEnabled => false;

	/// <summary>
	/// Модификатор объекта запроса для операции удаления всего множества элементов. По умолчанию не задана.
	/// Определите поведение в производном классе, чтобы ограничить выборку удаляемых элементов.
	/// </summary>
	protected virtual Func<IQueryable<TEntity>, IQueryable<TEntity>>? DeleteAllQueryHandler { get; } = null;


	/// <summary>
	/// Возвращает сервис, переопределяющий стандартную реализацию операций над множествами, и содержащий дополнительные операции
	/// для данного конкретного случая, преобразованный в указанный тип.
	/// </summary>
	/// <typeparam name="T">Тип сервиса, переопределяющего стандартную реализацию операций над множествами и содержащего дополнительные операции
	/// для данного конкретного случая.</typeparam>
	protected T GetCustomService<T>() where T: class, IDataOperation => CustomDataOperationsService is not null
		? (T)CustomDataOperationsService
		: throw new InvalidCastException("Попытка преобразования пустого значения в сервис, переопределяющий стандартную реализацию операций над множествами " +
            "и содержащий дополнительные операции для данного конкретного случая.");

	/// <summary>
	/// Действие операции создания нового элемента.
	/// </summary>
	/// <param name="dto">DTO с данными, необходимыми для создания нового элемента.</param>
	/// <returns>DTO созданного элемента в случае успеха операции.</returns>
	[HttpPost]
	public virtual async Task<IActionResult> Create([FromBody] TDto dto, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(dto, nameof(dto));


		var operationOverriding = CustomDataOperationsService as ICreateDataOperation<TEntity, TKey>;

		if (DefaultDataOperationsService is null && operationOverriding is null)
			return NotFound();

		if (dto is null)
			return BadRequest("У запроса на создание нового ресурса пустое тело.");

		if (CustomDataOperationsService is IValidateDataOperation<TDto, TEntity, TKey> validator)
			await validator.Validate(dto, null, ct);

		var entity = Mapper.Map<TEntity>(dto);
		TEntity? createdEntity = operationOverriding is null
			? DefaultDataOperationsService is not null
				? await DefaultDataOperationsService.Create(entity, BeforeCreateOperation, ct)
				: null
			: await operationOverriding.Create(entity, ct);

		if (createdEntity is null)
			throw new Exception("Операция создания нового ресурса завершилась без ошибок, но возвращаемый результат пуст.");

		if (createdEntity.Id.Equals(default))
			throw new ConflictException("Операция создания нового ресурса завершилась без ошибок, но возвращаемый результат имеет пустой идентификатор.");

		return Created(CalculateUrl(Convert.ToString(createdEntity.Id) ?? string.Empty), Mapper.Map<TDto>(createdEntity));
	}

	/// <summary>
	/// Действие операции удаления элемента с указанным идентификатором.
	/// </summary>
	/// <param name="id">Идентификатор элемента.</param>
	[HttpDelete("{id}")]
	public virtual async Task<IActionResult> Delete(TKey id, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(id, nameof(id));


		if (CustomDataOperationsService is not IDeleteDataOperation<TKey> operationOverriding)
		{
			if (DefaultDataOperationsService is null)
				return NotFound();

			await DefaultDataOperationsService.Delete(id, BeforeDeleteOperation, ct);
		}
		else
		{
			await operationOverriding.Delete(id, ct);
		}

		return NoContent();
	}

	/// <summary>
	/// Действие операции удаления всех элементов.
	/// </summary>
	[HttpDelete]
	public virtual async Task<IActionResult> DeleteAll(CancellationToken ct = default)
	{
		if (!DeleteAllOperationEnabled)
			return NotFound();

		if (CustomDataOperationsService is not IDeleteAllDataOperation operationOverriding)
		{
			if (DefaultDataOperationsService is null)
				return NotFound();

			await DefaultDataOperationsService.DeleteAll(DeleteAllQueryHandler, ct);
		}
		else
		{
			await operationOverriding.DeleteAll(ct);
		}

		return NoContent();
	}

	/// <summary>
	/// Действие операции запроса элемента по его идентификатору.
	/// </summary>
	/// <param name="id">Идентификатор элемента.</param>
	[HttpGet("{id}")]
	public virtual async Task<IActionResult> Get(TKey id, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(id, nameof(id));


		var result = await GetEntity(id, ct);

		return result is null ? throw new EntityNotFoundException() : (IActionResult)Ok(Mapper.Map<TDto>(result));
	}

	/// <summary>
	/// Действие операции запроса множества элементов.
	/// </summary>
	/// <param name="command">Команда, определяющая параметры объекта запроса.</param>
	[HttpGet]
	public virtual async Task<IActionResult> GetAll([FromQuery] QueryCommand? command, CancellationToken ct = default)
	{
		IEnumerable<TEntity> result;
		if (CustomDataOperationsService is not IGetAllDataOperation<TEntity, TKey> operationOverriding)
		{
			if (DefaultDataOperationsService is null)
				return NotFound();

			result = await DefaultDataOperationsService.GetAll(
				command,
				(CustomDataOperationsService as IChangeQueryDataOperation<TEntity>)?.GetQueryHandler(),
				ct);
		}
		else
		{
			result = await operationOverriding.GetAll(command, ct);
		}

		return Ok(Mapper.Map<IEnumerable<TDto>>(result));
	}

	/// <summary>
	/// Действие операции запроса количества элементов.
	/// </summary>
	/// <param name="command">Команда, определяющая параметры объекта запроса.</param>
	[HttpGet("total")]
	public virtual async Task<IActionResult> GetTotal([FromQuery] QueryCommand? command, CancellationToken ct = default)
	{
		int result;
		if (CustomDataOperationsService is not IGetTotalDataOperation<TEntity, TKey> operationOverriding)
		{
			if (DefaultDataOperationsService is null)
				return NotFound();

			result = await DefaultDataOperationsService.GetTotal(
				command,
				(CustomDataOperationsService as IChangeQueryDataOperation<TEntity>)?.GetQueryHandler(),
				ct);
		}
		else
		{
			result = await operationOverriding.GetTotal(command, ct);
		}

		return Ok(result);
	}

	/// <summary>
	/// Действие операции обновления существующего элемента.
	/// </summary>
	/// <param name="dto">DTO с данными, необходимыми для обновления нового элемента.</param>
	/// <returns>DTO обновлённого элемента в случае успеха операции.</returns>
	[HttpPut("{id}")]
	public virtual async Task<IActionResult> Update(TKey id, [FromBody] TDto dto, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNull(id, nameof(id));
		VariablesChecker.CheckIsNotNull(dto, nameof(dto));


		var operationOverriding = CustomDataOperationsService as IUpdateDataOperation<TEntity, TKey>;

		if (DefaultDataOperationsService is null && operationOverriding is null)
			return NotFound();

		if (dto is null)
			return BadRequest("У запроса на изменение существующего ресурса пустое тело.");

		var entity = await GetEntity(id, ct);

		if (CustomDataOperationsService is IValidateDataOperation<TDto, TEntity, TKey> validator)
			await validator.Validate(dto, entity, ct);

		Mapper.Map(dto, entity);

		TEntity? updatedEntity = operationOverriding is null
			? DefaultDataOperationsService is not null
				? await DefaultDataOperationsService.Update(entity, BeforeUpdateOperation, ct)
				: null
			: await operationOverriding.Update(entity, ct);

		if (updatedEntity is null)
			throw new ConflictException("Операция изменения существующего ресурса завершилась без ошибок, но возвращаемый результат пуст.");

		if (updatedEntity.Id.Equals(default))
			throw new ConflictException("Операция изменения существующего ресурса завершилась без ошибок, но возвращаемый результат имеет пустой идентификатор.");

		return Ok(Mapper.Map<TDto>(updatedEntity));
	}


	private async Task<TEntity> GetEntity(TKey key, CancellationToken ct)
	{
		TEntity? entity;
		if (CustomDataOperationsService is not IGetDataOperation<TEntity, TKey> operationOverriding)
		{
			if (DefaultDataOperationsService is null)
				throw new EntityNotFoundException();

			entity = await DefaultDataOperationsService.Get(
				key,
				(CustomDataOperationsService as IChangeQueryDataOperation<TEntity>)?.GetQueryHandler(),
				ct);
		}
		else
		{
			entity = await operationOverriding.Get(key, ct);
		}

		if (entity is null)
			throw new EntityNotFoundException();

		return entity;
	}
}

/// <inheritdoc/>
public abstract class DataControllerCore<TEntity, TDto, TKey> : DataControllerCore<TEntity, TDto, TKey, IDbContext>
	where TEntity : class, IEntity<TKey>
	where TDto : class, IDto<TKey>
	where TKey : IEquatable<TKey>
{
	protected DataControllerCore(
		IMapper mapper,
		IDataService<TEntity, TKey, IDbContext> defaultDataOperationsService,
		IDataOperation? customDataOperationsService = null)
		: base(mapper, defaultDataOperationsService, customDataOperationsService)
	{
	}

	protected DataControllerCore(IMapper mapper, IDataOperation customDataOperationsService)
		: base(mapper, customDataOperationsService)
	{
	}
}

/// <inheritdoc/>
public abstract class DataControllerCore<TEntity, TDto> : DataControllerCore<TEntity, TDto, int>
	where TEntity : class, IEntity
	where TDto : class, IDto
{
	private IMapper mapper;

	protected DataControllerCore(
		IMapper mapper,
		IDataService<TEntity, int, IDbContext> defaultDataOperationsService,
		IDataOperation? customDataOperationsService = null)
		: base(mapper, defaultDataOperationsService, customDataOperationsService)
	{
	}

	protected DataControllerCore(IMapper mapper, IDataOperation customDataOperationsService)
		: base(mapper, customDataOperationsService)
	{
	}
}

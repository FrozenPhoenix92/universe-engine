using AutoMapper;

using Universe.Core.Utils;

using Microsoft.AspNetCore.Mvc;

namespace Universe.Core.Infrastructure;

/// <summary>
/// Базовый класс API контроллера.
/// </summary>
[ApiController]
public abstract class ControllerCore : ControllerBase
{
	public ControllerCore(IMapper mapper)
	{
		VariablesChecker.CheckIsNotNull(mapper, nameof(mapper));

		Mapper = mapper;
	}


	/// <summary>
	/// Преобразователь объектов данных из DTO в модели и наоборот.
	/// </summary>
	protected IMapper Mapper { get; }


	/// <summary>
	/// Возвращает URL, состоящий из текущего домена и указанного хвоста.
	/// </summary>
	/// <param name="tail">Хвост, который должен быть добавлен в конец URL.</param>
	protected string CalculateUrl(string tail) => Request.Scheme + "://" + Request.Host + "/" + tail;

	protected NoContentResult NoContent(Action action)
	{
		VariablesChecker.CheckIsNotNull(action, nameof(action));

		action.Invoke();
		return NoContent();
	}

	protected async Task<NoContentResult> NoContent(Func<Task> action)
	{
		VariablesChecker.CheckIsNotNull(action, nameof(action));

		await action.Invoke();
		return NoContent();
	}
}

using Universe.Web.Configuration;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Universe.Web.Filters;

public class ValidateClientApplicationCodeHeaderFilterAttribute : Attribute, IActionFilter
{
	public const string ClientApplicationCodeHeaderName = "ApplicationCode";

	public void OnActionExecuted(ActionExecutedContext context)
	{
	}

	public void OnActionExecuting(ActionExecutingContext context)
	{
		if (!context.HttpContext.Request.Headers.TryGetValue(ClientApplicationCodeHeaderName, out var clientApplicationCodeHeader))
		{
			context.Result = new BadRequestObjectResult("Не задан код приложения.");
		}

		var clientApplicationSettings = context.HttpContext.RequestServices.GetService<IOptions<List<ClientApplicationSettings>>>()?.Value ?? new List<ClientApplicationSettings>();
		if (clientApplicationSettings.All(x => x.ApplicationCode != clientApplicationCodeHeader))
		{
			context.Result = new BadRequestObjectResult("Неверный код приложения.");
		}
	}
}

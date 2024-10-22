using Universe.Core.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Universe.Core.Exceptions;
using System.Collections.Concurrent;

namespace Universe.Web.Filters;

public class BlockedClientFilterAttribute : ActionFilterAttribute
{
	public static readonly ConcurrentDictionary<Guid, bool> DisabledClientIds = new ConcurrentDictionary<Guid, bool>();

	public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		if (Guid.TryParse(context.HttpContext.GetUserId(), out var clientId))
		{
			if (DisabledClientIds.ContainsKey(clientId))
			{
				throw new ForbiddenException("Пользователь заблокирован.");
			}
		}

		await next();
	}
}

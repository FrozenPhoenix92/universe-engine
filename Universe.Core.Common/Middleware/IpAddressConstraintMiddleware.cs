using Universe.Core.Common.Services;
using Universe.Core.Membership;
using Universe.Core.Membership.Model;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using System.Net;

namespace Universe.Core.Common.Middleware;

public class IpAddressConstraintMiddleware
{
	private readonly RequestDelegate _next;

	public IpAddressConstraintMiddleware(RequestDelegate next) => _next = next;

	public async Task Invoke(
		HttpContext context,
		UserManager<User> userManager,
		IIpAddressConstraintService ipAddressesConstraintService)
	{
		var user = await userManager.GetUserAsync(context.User);
		var remoteIpAddress = context.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "0.0.0.1";

		if (user is not null && await userManager.IsInRoleAsync(user, CoreRoles.SuperAdminRoleName) ||
			context.Connection.RemoteIpAddress is not null &&
			(remoteIpAddress == "0.0.0.1" ||
			await ipAddressesConstraintService.IsAllowedRequest(remoteIpAddress, context.Request.Path.ToString())))
		{
			await _next.Invoke(context);
			return;
		}

		context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
		return;
	}
}

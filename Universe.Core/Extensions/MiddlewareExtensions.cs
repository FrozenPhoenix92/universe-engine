using Universe.Core.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Universe.Core.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseHttpToHttpsRedirectMiddleware(this IApplicationBuilder builder) =>
        builder.UseMiddleware<HttpToHttpsRedirectMiddleware>();

    public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder) =>
        builder.UseMiddleware<ErrorHandlingMiddleware>();
}

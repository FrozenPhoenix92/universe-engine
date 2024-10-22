using Universe.Core.Exceptions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Text.Json;

namespace Universe.Core.Middleware;

public class ErrorHandlingMiddleware
{
	private const string FailedRequestItemKey = "ErrorHandlingMiddleware_FailedRequest";
	private const string StatusCodeItemKey = "ErrorHandlingMiddleware_StatusCode";
	private const string ContentTypeItemKey = "ErrorHandlingMiddleware_ContentType";
	private const string InternalErrorProdMessage = "¬нутренн€€ ошибка сервера.";

	private readonly RequestDelegate _next;
	private readonly IWebHostEnvironment _hostingEnvironment;


	public ErrorHandlingMiddleware(RequestDelegate next, IWebHostEnvironment hostingEnvironment)
	{
		_next = next;
		_hostingEnvironment = hostingEnvironment;
	}


	public async Task Invoke(HttpContext context)
	{
		try
		{
			context.Response.OnStarting(() =>
			{
				if (context.Items.TryGetValue(FailedRequestItemKey, out var isFailedRequest) && isFailedRequest is true)
				{
					context.Response.ContentType = (string)(context.Items[ContentTypeItemKey] ?? string.Empty);
					context.Response.StatusCode = (int)(context.Items[StatusCodeItemKey] ?? 0);
				}
				return Task.CompletedTask;
			});

			await _next(context);
		}
		catch (Exception ex)
		{
			await HandleException(context, ex);
		}
	}


	private static string FormatValidationException(ValidationException ex)
	{
		ValidationProblemDetails GetValidationResult(ValidationException e)
		{
			if (e.ValidationResult.MemberNames is null || !e.ValidationResult.MemberNames.Any())
			{
				return new ValidationProblemDetails(
					new Dictionary<string, string[]> { { "ќшибка валидации", new[] { e.ValidationResult.ErrorMessage ?? string.Empty } } });
			}
			return new ValidationProblemDetails(
				e.ValidationResult.MemberNames.ToDictionary(key => key, value => new[] { e.ValidationResult.ErrorMessage ?? string.Empty }));
		}
		return JsonSerializer.Serialize(GetValidationResult(ex));
	}

	private async Task HandleException(HttpContext context, Exception ex)
	{
		var isDevelopment = _hostingEnvironment.IsDevelopment();

		int code;
		string message = ex.Message;

		switch (ex)
		{
			case ForbiddenException _: code = StatusCodes.Status403Forbidden; break;
			case EntityNotFoundException _: code = StatusCodes.Status404NotFound; break;
			case ConflictException _: code = StatusCodes.Status409Conflict; break;
			case ApiException _: code = StatusCodes.Status400BadRequest; break;
			case BusinessException _: code = StatusCodes.Status400BadRequest; break;
			case ValidationException validationException:
				code = StatusCodes.Status400BadRequest;
				message = FormatValidationException(validationException);
				break;
			default:
				code = StatusCodes.Status500InternalServerError;
				message = isDevelopment ? ex.Message : InternalErrorProdMessage;
				break;
		}

		context.Response.ContentType = MediaTypeNames.Application.Json;
		context.Response.StatusCode = code;
		await context.Response.WriteAsync(message);
	}
}

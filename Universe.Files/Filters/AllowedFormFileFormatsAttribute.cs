using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Universe.Files.Filters;

/// <summary>
/// Определяет допустимые MIME типы запроса в параметре <see cref="Microsoft.AspNetCore.Http.IFormFileCollection"/> действия.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AllowedFormFileFormatsAttribute : Attribute, IActionFilter
{
    private readonly string[] _allowedMimeTypes;
    private readonly long _maxFileSizeBytes = 0;


    /// <summary>
    /// Создаёт новый экземпляр класса <see cref="AllowedFormFileFormatsAttribute" /> с допустимыми MIME типами.
    /// </summary>
    /// <param name="mimeTypes">Допустимые MIME типы.</param>
    public AllowedFormFileFormatsAttribute(params string[] mimeTypes) =>
        _allowedMimeTypes = mimeTypes;

    /// <summary>
    /// Создаёт новый экземпляр класса <see cref="AllowedFormFileFormatsAttribute" /> с допустимыми MIME типами и максимально допустимым размером файла.
    /// </summary>
    /// <param name="maxFileSizeBytes">Максимально допустимый размер файла в байтах.</param>
    /// <param name="mimeTypes">Допустимые MIME типы.</param>
    public AllowedFormFileFormatsAttribute(long maxFileSizeBytes, params string[] mimeTypes) : this(mimeTypes) =>
        _maxFileSizeBytes = maxFileSizeBytes;


    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Request.Form.Files.Any(fileItem => !_allowedMimeTypes.Contains(fileItem.ContentType)))
        {
            context.Result = new UnsupportedMediaTypeResult();
        }
        else
        {
            if (_maxFileSizeBytes > 0 && context.HttpContext.Request.Form.Files.Any(fileItem => fileItem.Length > _maxFileSizeBytes))
            {
                context.Result = new BadRequestObjectResult("Размер файла превышает допустимое значение.");
            }
        }
    }
}

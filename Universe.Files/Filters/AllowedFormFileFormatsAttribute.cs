using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Universe.Files.Filters;

/// <summary>
/// ���������� ���������� MIME ���� ������� � ��������� <see cref="Microsoft.AspNetCore.Http.IFormFileCollection"/> ��������.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AllowedFormFileFormatsAttribute : Attribute, IActionFilter
{
    private readonly string[] _allowedMimeTypes;
    private readonly long _maxFileSizeBytes = 0;


    /// <summary>
    /// ������ ����� ��������� ������ <see cref="AllowedFormFileFormatsAttribute" /> � ����������� MIME ������.
    /// </summary>
    /// <param name="mimeTypes">���������� MIME ����.</param>
    public AllowedFormFileFormatsAttribute(params string[] mimeTypes) =>
        _allowedMimeTypes = mimeTypes;

    /// <summary>
    /// ������ ����� ��������� ������ <see cref="AllowedFormFileFormatsAttribute" /> � ����������� MIME ������ � ����������� ���������� �������� �����.
    /// </summary>
    /// <param name="maxFileSizeBytes">����������� ���������� ������ ����� � ������.</param>
    /// <param name="mimeTypes">���������� MIME ����.</param>
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
                context.Result = new BadRequestObjectResult("������ ����� ��������� ���������� ��������.");
            }
        }
    }
}

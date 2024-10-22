using Universe.Core.Exceptions;
using Universe.Files.Dto;

namespace BBWM.FileStorage;

public sealed class FilesUploadingException : DataException
{
    public FilesUploadingException(FilesUploadingResult uploadingResult)
        : base("При загрузке следующих файлов произошла ошибка: " +
              $"{string.Join(' ', uploadingResult.FailedUploadedFileNames ?? Array.Empty<string>())}") =>
        UploadingResult = uploadingResult;


    public FilesUploadingResult UploadingResult { get; }
}

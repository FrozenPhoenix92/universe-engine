using Universe.Files.Dto;
using Microsoft.AspNetCore.Http;

namespace Universe.Files.Services;

public interface IFileOperationService
{
	Task<FilesUploadingResult> UploadFiles(IFormFile[] files, Dictionary<string, string?> additionalData, CancellationToken cancellationToken = default);
}

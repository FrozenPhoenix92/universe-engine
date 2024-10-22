using AutoMapper;

using BBWM.FileStorage;

using Universe.Core.Data;
using Universe.Files.Dto;
using Universe.Files.Model;

using Microsoft.AspNetCore.Http;

using Serilog;

namespace Universe.Files.Services;

public class FileOperationService : IFileOperationService
{
	private readonly IDbContext _dataContext;
	private readonly IFileStorageService _fileStorageService;
	private readonly IMapper _mapper;


	public FileOperationService(
		IMapper mapper,
		IDbContext dataContext,
		IFileStorageService fileStorageService)
	{
		_dataContext = dataContext;
		_fileStorageService = fileStorageService;
		_mapper = mapper;
	}


	public async Task<FilesUploadingResult> UploadFiles(IFormFile[] files, Dictionary<string, string?> additionalData, CancellationToken cancellationToken = default)
	{
		if (files is null) throw new ArgumentNullException(nameof(files));
		if (!files.Any()) throw new AggregateException("Не найдено файлов для загрузки.");

		// Removing old files and DB records based on existing saved operation for the user
		if (additionalData.ContainsKey("user_id") && additionalData.ContainsKey("operation_name"))
		{
			var fileDetails = GetFileDetailsByUserAndOperationName(additionalData["user_id"], additionalData["operation_name"]);
			foreach (var fileDetailsItem in fileDetails)
			{
				await DeleteFileDetails(fileDetailsItem, cancellationToken);
			}
		}

		var result = new FilesUploadingResult();
		var failedFilesCount = 0;
		foreach (var file in files)
		{
			try
			{
				result.SuccessfullyUploadedFiles.Add(await SaveFile(file, additionalData, cancellationToken));
			}
			catch
			{
				failedFilesCount++;
				result.FailedUploadedFileNames.Add(file.FileName);
			}
		}

		if (failedFilesCount > 0)
		{
			result.UploadingStatus = failedFilesCount == files.Length
				? FilesUploadingStatus.Failed
				: FilesUploadingStatus.PartialSuccess;
		}

		if (result.UploadingStatus != FilesUploadingStatus.Success)
		{
			throw new FilesUploadingException(result);
		}

		return result;
	}


	private static bool IsImage(IFormFile file) => file.ContentType.Contains("image");

	private async Task<bool> DeleteFileDetails(FileDetails fileDetails, CancellationToken cancellationToken)
	{
		bool filesResult = false;

		try
		{
			filesResult = await _fileStorageService.DeleteFile($"{fileDetails.RelatedDataKey}.{fileDetails.Extension}", cancellationToken);
		}
		catch (Exception ex) 
		{
			Log.Error($"Не удалось удалить файл: {ex.Message}");
		}

		_dataContext.Set<FileDetails>().Remove(fileDetails);
		var removedRecordsCount = await _dataContext.SaveChangesAsync(cancellationToken);

		return removedRecordsCount > 0 && filesResult;
	}

	private FileDetails[] GetFileDetailsByUserAndOperationName(string? userId, string? operationName) =>
		_dataContext.Set<FileDetails>()
			.Where(item => item.RelatedResponsibleKey != null && item.RelatedResponsibleKey == userId &&
				item.RelatedOperationKey != null && item.RelatedOperationKey == operationName)
			.ToArray();

	private async Task<FileDetailsDto> SaveFile(IFormFile file, Dictionary<string, string?> additionalData, CancellationToken ct = default)
	{
		int? ExtractIntFromAdditionalData(string dataKey)
		{
			int? res = null;
			if (additionalData.TryGetValue(dataKey, out var resStr) && int.TryParse(resStr, out var resVal))
			{
				res = resVal;
			}
			return res;
		}

		DateTimeOffset? ExtractDateFromAdditionalData(string dataKey)
		{
			DateTimeOffset? res = null;
			if (additionalData.TryGetValue(dataKey, out var resStr) && DateTimeOffset.TryParse(resStr, out var resVal))
			{
				res = resVal;
			}
			return res;
		}


		var key = Guid.NewGuid().ToString();
		var userId = additionalData.TryGetValue("user_id", out string? value) ? value : null;
		var operationName = additionalData.ContainsKey("operation_name") ? additionalData["operation_name"] : string.Empty;
		var lastModified = ExtractDateFromAdditionalData("last_modified").GetValueOrDefault(DateTimeOffset.UtcNow);
		var extension = Path.GetExtension(file.FileName).Remove(0, 1);
		// var size = file.Length;
		// var isImage = IsImage(file);
		// string thumbnailKey = null;

		// File saving
		using (var fileStream = file.OpenReadStream())
		{
			// TODO: SixLabors library performs resizing with a bug (making transparent background black)
			/*if (isImage && SupportedMimeType(file))
            {
                var maxSize = ExtractIntFromAdditionalData("max_size").GetValueOrDefault(1500);
                var thumbnailSize = ExtractIntFromAdditionalData("thumbnail_size").GetValueOrDefault(400);
                var degree = ExtractIntFromAdditionalData("degree").GetValueOrDefault(0);
                var scaleX = ExtractIntFromAdditionalData("scaleX").GetValueOrDefault(1);
                var scaleY = ExtractIntFromAdditionalData("scaleY").GetValueOrDefault(1);

                using (var imageStream = ReduceTooLargeImage(fileStream, maxSize, degree, scaleX, scaleY))
                {
                    size = imageStream.Length;
                    await _fileStorageProvider.UploadFile(imageStream, $"{key}.{extension}", cancellationToken);
                }
                using (var thumbnailStream = CreateThumbnailImage(fileStream, thumbnailSize, degree, scaleX, scaleY))
                {
                    try
                    {
                        thumbnailKey = Guid.NewGuid().ToString();
                        await _fileStorageProvider.UploadFile(thumbnailStream, $"{thumbnailKey}.{extension}", cancellationToken);
                    }
                    catch
                    {
                        await _fileStorageProvider.DeleteFile(key, cancellationToken);
                        throw;
                    }
                }
            }
            else
            {
                await _fileStorageProvider.UploadFile(fileStream, $"{key}.{extension}", cancellationToken);
            }*/

			await _fileStorageService.UploadFile(fileStream, $"{key}.{extension}", ct);
		}

		try
		{
			// Creating DB record
			var dbDetails = new FileDetails
			{
				Created = DateTimeOffset.UtcNow,
				Name = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{key}",
				Extension = extension,
				LatestUpdated = lastModified,
				RelatedResponsibleKey = userId,
				RelatedOperationKey = operationName!,
				RelatedDataKey = key
			};
			await _dataContext.Set<FileDetails>().AddAsync(dbDetails, ct);
			await _dataContext.SaveChangesAsync(ct);

			var res = _mapper.Map<FileDetailsDto>(dbDetails);

			return res;
		}
		catch (Exception)
		{
			// If the record creation failed we should remove unbound files
			await _fileStorageService.DeleteFile($"{key}.{extension}", ct);

			throw;
		}
	}
}

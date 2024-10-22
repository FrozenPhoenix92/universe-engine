using Universe.Core.Utils;
using Universe.Files.Configuration;
using Universe.Files.Dto;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Universe.Files.Services;

public class LocalFolderFileStorageService : IFileStorageService
{
	private readonly string _path;


	public LocalFolderFileStorageService(IWebHostEnvironment hostingEnvironment, IOptions<FileStorageSettings> fileStorageSettingsOptions)
	{
		VariablesChecker.CheckIsNotNull(hostingEnvironment, nameof(hostingEnvironment));

		_path = Path.Combine(hostingEnvironment.WebRootPath, fileStorageSettingsOptions?.Value?.Folder ?? "upload");
	}

	public Task<bool> DeleteFile(string key, CancellationToken cancellationToken = default)
	{
		return Task.Run(() =>
		{
			File.Delete(GetFilePath(key));
			return true;
		});
	}

	public async Task<byte[]> DownloadFile(string key, CancellationToken cancellationToken = default)
	{
		using var memoryStream = new MemoryStream();
		using (var fileStream = File.OpenRead(GetFilePath(key)))
		{
			fileStream.Seek(0, SeekOrigin.Begin);
			await fileStream.CopyToAsync(memoryStream);
		}

		return memoryStream.ToArray();
	}

	public Task<StorageFileData?> GetFile(string key, CancellationToken cancellationToken = default)
	{
		return Task.Run(() =>
		{
			var fileInfo = new FileInfo(GetFilePath(key));
			return !fileInfo.Exists
				? null
				: new StorageFileData
				{
					Key = key,
					IsImage = true,
					LastModifiedDate = fileInfo.LastWriteTimeUtc,
					Size = fileInfo.Length,
					Url = $"data/images/{key}"
				};
		}, cancellationToken);
	}

	public async Task<StorageFileData?> UploadFile(Stream stream, string key, CancellationToken cancellationToken = default)
	{
		if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);

		var filePath = GetFilePath(key);
		using (var fileStream = File.Create(filePath))
		{
			stream.Seek(0, SeekOrigin.Begin);
			await stream.CopyToAsync(fileStream);
		}

		return new StorageFileData
		{
			Key = key,
			Url = filePath,
		};
	}


	private string GetFilePath(string key) => Path.Combine(_path, key);
}

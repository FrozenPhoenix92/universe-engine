using Universe.Files.Dto;

namespace Universe.Files.Services;

public interface IFileStorageService
{
	Task<bool> DeleteFile(string key, CancellationToken cancellationToken = default);

	Task<byte[]> DownloadFile(string key, CancellationToken cancellationToken = default);

	Task<StorageFileData?> GetFile(string key, CancellationToken cancellationToken = default);

	Task<StorageFileData?> UploadFile(Stream stream, string key, CancellationToken cancellationToken = default);
}

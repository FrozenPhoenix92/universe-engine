namespace Universe.Files.Dto;


public enum FilesUploadingStatus
{
	Failed,
	PartialSuccess,
	Success
}

public class FilesUploadingResult
{
	public FilesUploadingResult()
	{
		SuccessfullyUploadedFiles = new List<FileDetailsDto>();
		FailedUploadedFileNames = new List<string>();
		UploadingStatus = FilesUploadingStatus.Success;
	}


	public IList<FileDetailsDto> SuccessfullyUploadedFiles { get; set; }

	public IList<string> FailedUploadedFileNames { get; set; }

	public FilesUploadingStatus UploadingStatus { get; set; }
}

namespace Universe.Files.Configuration;

public enum FileStorageType
{
	LocalFolder = 0
}

public class FileStorageSettings
{
	public FileStorageType StorageType { get; set; } = FileStorageType.LocalFolder;

	public string Folder { get; set; } = "upload";
}

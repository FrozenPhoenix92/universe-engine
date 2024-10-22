namespace Universe.Web.Configuration;

public class EnvironmentSettings
{
	public string DataFilesRootFolder { get; set; } = "data";

	public string? SpaControlPanelSourcePath { get; set; }

	public string? SpaControlPanelProxyServer { get; set; }

	public string[] SpaClientOrigins { get; set; } = Array.Empty<string>();
}

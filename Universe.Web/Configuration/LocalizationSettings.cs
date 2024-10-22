namespace Universe.Web.Configuration;

public class LocalizationSettings
{
	public string? ResourcesPath { get; set; }

	public IList<string> Cultures { get; set; } = new List<string>();
}

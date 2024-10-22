using Universe.Core.Infrastructure;

using System.Text.Json.Nodes;

namespace Universe.Core.Common.Dto;

public class AppSettingsSetDto : IDto
{
	public int Id { get; set; }

	public string? Name { get; set; }

	public bool ReadOnly { get; set; } = false;

	public JsonNode? Value { get; set; }
}

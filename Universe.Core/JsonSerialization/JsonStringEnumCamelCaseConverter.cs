using System.Text.Json;
using System.Text.Json.Serialization;

namespace Universe.Core.JsonSerialization;

public class JsonStringEnumCamelCaseConverter : JsonStringEnumConverter
{
	public JsonStringEnumCamelCaseConverter() : base(JsonNamingPolicy.CamelCase)
	{
	}
}

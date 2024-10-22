using Universe.Core.Exceptions;
using Universe.Core.QueryHandling.Filters;

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Universe.Core.QueryHandling;

public class FilterInfoBaseJsonConverter : JsonConverter<FilterInfoBase>
{
	public override void Write(Utf8JsonWriter writer, FilterInfoBase value, JsonSerializerOptions options) => throw new NotImplementedException();

	public override FilterInfoBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		using (var document = JsonDocument.ParseValue(ref reader))
		{
			var jsonElement = document.RootElement;

			string? filterTypeName = jsonElement.TryGetProperty("$type", out var typeJsonElement)
				? typeJsonElement.GetString()
				: throw new InvalidRequestDataException("Не удалось получить свойство '$type' у экземпляра класса 'FilterInfoBase'.");

			if (string.IsNullOrWhiteSpace(filterTypeName))
				throw new InvalidRequestDataException("Значение свойства '$type' экземпляра класса 'FilterInfoBase' не задано.");

			var filtersAssembly = Assembly.GetAssembly(typeof(FilterInfoBase));

			if (filtersAssembly is null)
				throw new ConflictException("Не удалось найти сборку, которой принадлежит класс 'FilterInfoBase'.");

			var filterType = filtersAssembly
				.GetTypes()
				.FirstOrDefault(a => a.GetTypeInfo().IsSubclassOf(
					typeof(FilterInfoBase)) &&
					!a.IsAbstract &&
					string.Equals(a.Name, $"{filterTypeName}Filter", StringComparison.InvariantCultureIgnoreCase));

			if (filterType is null)
				throw new InvalidRequestDataException($"Класс фильтра с именем '{filterTypeName}Filter' не существует.");

			if (jsonElement.Deserialize(filterType, options) is not FilterInfoBase filter)
				throw new InvalidRequestDataException("Не удалось десериализовать объект фильтра.");

			return filter;
		}
	}

	public override bool CanConvert(Type objectType) => typeof(FilterInfoBase).IsAssignableFrom(objectType);
}

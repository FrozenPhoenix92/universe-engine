﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace Universe.Core.JsonSerialization
{
    public class ObjectConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True)
                return true;

            if (reader.TokenType == JsonTokenType.False)
                return false;

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out long l))
                    return l;

                return reader.GetDouble();
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                if (reader.TryGetDateTime(out DateTime datetime))
                    return datetime;

                return reader.GetString();
            }

            return JsonElement.ParseValue(ref reader);
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options) =>
			JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}

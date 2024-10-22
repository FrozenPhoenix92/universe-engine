using System.Text.Json;

namespace Universe.Core.JsonSerialization
{
    public static class JsonSerializerOptionsProvider
    {
        private static readonly JsonSerializerOptions _defaultOptions =
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        private static JsonSerializerOptions? _pureOptions;
        private static JsonSerializerOptions? _fullOptions;


        public static JsonSerializerOptions GetOptions(bool includeCustomConverters = true)
        {
            return includeCustomConverters
                ? _fullOptions ?? _defaultOptions
                : _pureOptions ?? _defaultOptions;
        }

        public static void SetOptions(JsonSerializerOptions pureOptions, JsonSerializerOptions fullOptions)
        {
            _pureOptions = pureOptions;
            _fullOptions = fullOptions;
        }
    }
}


using System.Text.Json;
using System.Text.Json.Serialization;


namespace MiSmart.Infrastructure.Constants
{
    public class JsonSerializerDefaultOptions
    {
        public static JsonSerializerOptions CamelOptions = new JsonSerializerOptions { DictionaryKeyPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };
    }
}
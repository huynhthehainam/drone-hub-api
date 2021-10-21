
using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonOptions
{
    public static JsonSerializerOptions CamelOptions = new JsonSerializerOptions { DictionaryKeyPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };
}
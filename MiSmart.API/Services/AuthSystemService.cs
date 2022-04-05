
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MiSmart.Infrastructure.Constants;

namespace MiSmart.API.Services
{
    public class CheckUserIDExistsResponse
    {
        public class CheckUserIDExistsData
        {
            public Boolean Exists { get; set; }
        }
        public CheckUserIDExistsData Data { get; set; }
    }
    public class AuthSystemSettings
    {
        public String Url { get; set; }
    }
    public class AuthSystemService
    {
        private AuthSystemSettings settings;
        private IHttpClientFactory httpClientFactory;
        public AuthSystemService(IOptions<AuthSystemSettings> options, IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            this.settings = options.Value;
        }
        public Boolean CheckUserIDExists(Int64 userID)
        {
            var client = httpClientFactory.CreateClient();
            StringContent content = new StringContent(JsonSerializer.Serialize(new { UserID = userID }, JsonSerializerDefaultOptions.CamelOptions), Encoding.UTF8, "application/json");
            var response = client.PostAsync($"{settings.Url}/Auth/CheckUserIDExists", content).Result;
            if (response.IsSuccessStatusCode)
            {
                String responseString = response.Content.ReadAsStringAsync().Result;
                CheckUserIDExistsResponse checkUserIDExistsResponse = JsonSerializer.Deserialize<CheckUserIDExistsResponse>(responseString, JsonSerializerDefaultOptions.CamelOptions);
                return checkUserIDExistsResponse.Data.Exists;
            }
            return false;
        }

    }
}
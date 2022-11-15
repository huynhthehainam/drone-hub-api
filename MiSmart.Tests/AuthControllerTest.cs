

using MiSmart.API;
using System.Net.Http.Json;
using MiSmart.Infrastructure.Constants;
using System.Net;


namespace MiSmart.Tests;
public sealed class AuthControllerTest : IClassFixture<CustomWebApplicationFactory<MiSmart.API.Startup>>
{
    public sealed class GenerateDeviceTokenData
    {
        public String? AccessToken { get; set; }
    }
    public sealed class GenerateDeviceTokenResponse
    {
        public GenerateDeviceTokenData? Data { get; set; }
    }
    private readonly CustomWebApplicationFactory<MiSmart.API.Startup> factory;
    public AuthControllerTest(CustomWebApplicationFactory<Startup> factory)
    {
        this.factory = factory;
    }
    [Fact]
    public async Task GetDeviceAccessTokenAndReturnAccessToken()
    {
        var client = factory.CreateClient();

        var resp = await client.PostAsJsonAsync("/auth/generatedevicetoken", new
        {
            DeviceToken = "S3XEsPNgYetJQJLebPheN2HSC2WTsuxE"
        });

        GenerateDeviceTokenResponse? deviceTokenResponse = await resp.Content.ReadFromJsonAsync<GenerateDeviceTokenResponse>(JsonSerializerDefaultOptions.CamelOptions);

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.NotNull(deviceTokenResponse);
        Assert.NotNull(deviceTokenResponse?.Data);
        Assert.NotNull(deviceTokenResponse?.Data?.AccessToken);
    }
}

using System.Net;
using System.Text.Json;

namespace MiSmart.Tests;
[Collection("Sequential")]
public class HomePageTest : IClassFixture<CustomWebApplicationFactory<MiSmart.API.Startup>>
{
    private readonly CustomWebApplicationFactory<MiSmart.API.Startup> factory;
    public HomePageTest(CustomWebApplicationFactory<MiSmart.API.Startup> factory)
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "google_services.json");
        this.factory = factory;
    }
    [Fact]
    public async Task GetHomePageAndReturnAllowedServices()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var resp = await client.GetAsync("/");




        var contentStr = await resp.Content.ReadAsStringAsync();
        Console.WriteLine($"contentStr {contentStr}");
        JsonDocument jsonDocument = JsonDocument.Parse(contentStr);
        var root = jsonDocument.RootElement;

        var data = root.GetProperty("data");
        var allowedVersions = data.GetProperty("allowedVersions").EnumerateArray();
        var env = data.GetProperty("env").GetString();


        // Assert
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.NotEmpty(allowedVersions);
        Assert.Equal("Testing", env);
    }
}
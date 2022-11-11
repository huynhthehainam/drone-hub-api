using System.Text.Json;

namespace MiSmart.Tests;
[CollectionDefinition("Non-Parallel Collection", DisableParallelization = true)]
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
        var homePageResponse = await client.GetAsync("/");



        homePageResponse.EnsureSuccessStatusCode();
        var contentStr = await homePageResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"contentStr {contentStr}");
        JsonDocument jsonDocument = JsonDocument.Parse(contentStr);
        var root = jsonDocument.RootElement;

        var data = root.GetProperty("data");
        var allowedVersions = data.GetProperty("allowedVersions").EnumerateArray();


        // Assert
        Assert.Empty(allowedVersions);
        // Assert.Equal(HttpStatusCode.OK, homePage.StatusCode);

    }
}
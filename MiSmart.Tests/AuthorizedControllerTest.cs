
using Microsoft.Extensions.DependencyInjection;
using MiSmart.Infrastructure.Services;

namespace MiSmart.Tests;


public abstract class AuthorizedControllerTest<TProgram> : IClassFixture<CustomWebApplicationFactory<TProgram>> where TProgram : class
{
    protected readonly CustomWebApplicationFactory<TProgram> factory;
    public AuthorizedControllerTest(CustomWebApplicationFactory<TProgram> factory)
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "google_services.json");
        this.factory = factory;
    }
    public String CreateTestingJWTToken(Infrastructure.ViewModels.UserCacheViewModel? user = null)
    {
        if (user == null)
        {
            user = new Infrastructure.ViewModels.UserCacheViewModel
            {
                ID = 1,
                IsAdmin = true,
                IsActive = true,
                RoleID = 1,
                Email = "test@mismart.ai",
                Username = "test@mismart.ai",
            };
        }

        using (var scope = factory.Services.CreateScope())
        {
            JWTService service = scope.ServiceProvider.GetRequiredService<JWTService>();
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(120);
            return service.GenerateAccessToken(user, accessTokenExpiration);
        }
    }
    public HttpClient CreateAuthorizedClient(Infrastructure.ViewModels.UserCacheViewModel? user = null)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", CreateTestingJWTToken(user));
        return client;
    }
}

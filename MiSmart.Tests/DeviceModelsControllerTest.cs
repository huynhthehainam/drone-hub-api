using System.Net;
using System.Net.Http.Json;
using MiSmart.API;
using MiSmart.Infrastructure.Constants;

namespace MiSmart.Tests;

public sealed class DeviceModelsControllerTest : AuthorizedControllerTest<MiSmart.API.Startup>
{
    public DeviceModelsControllerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
    {
    }
    [Fact]
    public async Task CreateDeviceModelWithRoleAdminAndReturnCreatedObject()
    {
        var client = CreateAuthorizedClient();

        var resp = await client.PostAsJsonAsync("/devicemodels", new { Name = "asfsafas", Type = "Pressure" }, JsonSerializerDefaultOptions.CamelOptions);
        CreateResponseTest<Int32>? createResponse = await resp.Content.ReadFromJsonAsync<CreateResponseTest<Int32>>(JsonSerializerDefaultOptions.CamelOptions);

        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        Assert.NotNull(createResponse);
        Assert.NotNull(createResponse?.Data);
        Assert.True(createResponse?.Data?.ID > 0);

    }
    [Fact]
    public async Task CreateDeviceModelWithRoleStaffAndReturnForbiddenMessage()
    {
        var client = CreateAuthorizedClient(GenerateStaffUser());
        var resp = await client.PostAsJsonAsync("/devicemodels", new { Name = "asfsafas", Type = "Pressure" }, JsonSerializerDefaultOptions.CamelOptions);

        Assert.Equal(resp.StatusCode, HttpStatusCode.Forbidden);
    }
}

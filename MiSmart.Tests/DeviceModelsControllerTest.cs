using System.Net;
using System.Net.Http.Json;
using MiSmart.API;
using MiSmart.Infrastructure.Constants;
using MiSmart.DAL.ViewModels;

namespace MiSmart.Tests;

public sealed class DeviceModelsControllerTest : AuthorizedControllerTest<MiSmart.API.Startup>
{
    public DeviceModelsControllerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
    {
    }
    private readonly Int32 testingDeviceModelID = 1;
    public Object GenerateCreatingCommand()
    {
        return new { Name = "asfsafas", Type = "Pressure", SprayingModes = new List<String>() { "1 pump" } };
    }
    [Fact]
    public async Task CreateDeviceModelWithRoleAdminAndReturnCreatedObject()
    {
        var client = CreateAuthorizedClient();

        var resp = await client.PostAsJsonAsync("/devicemodels", GenerateCreatingCommand(), JsonSerializerDefaultOptions.CamelOptions);
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
        var resp = await client.PostAsJsonAsync("/devicemodels", GenerateCreatingCommand(), JsonSerializerDefaultOptions.CamelOptions);

        Assert.Equal(resp.StatusCode, HttpStatusCode.Forbidden);
    }
    [Fact]
    public async Task GetListDeviceModelAndReturnListDeviceModel()
    {
        var client = CreateAuthorizedClient();

        var resp = await client.GetAsync("/devicemodels?pageIndex=0&pageSize=5");

        ListResponseTest<SmallDeviceModelViewModel>? listResponse = await resp.Content.ReadFromJsonAsync<ListResponseTest<SmallDeviceModelViewModel>>(JsonSerializerDefaultOptions.CamelOptions);

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.NotNull(listResponse);
        Assert.NotNull(listResponse?.Data);
        Assert.NotEmpty(listResponse?.Data);
    }
    [Fact]
    public async Task PatchDeviceModelWithRoleAdminAndReturnUpdatedMessage()
    {
        var client = CreateAuthorizedClient();

        var resp = await client.PatchAsync($"/devicemodels/{testingDeviceModelID}", JsonContent.Create(new
        {
            Name = "update test"
        }, options: JsonSerializerDefaultOptions.CamelOptions));

        var data = await resp.Content.ReadAsStringAsync();
        UpdatedResponseTest? updatedResponse = await resp.Content.ReadFromJsonAsync<UpdatedResponseTest>(JsonSerializerDefaultOptions.CamelOptions);

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.NotNull(updatedResponse);
        Assert.NotNull(updatedResponse?.Message);
    }
    [Fact]
    public async Task PatchDeviceModelWithRoleStaffAndReturnForbiddenMessage()
    {

        var client = CreateAuthorizedClient(GenerateStaffUser());

        var resp = await client.PatchAsync($"/devicemodels/{testingDeviceModelID}", JsonContent.Create(new
        {
            Name = "update test"
        }, options: JsonSerializerDefaultOptions.CamelOptions));

        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }
}

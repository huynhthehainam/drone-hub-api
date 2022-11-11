

using System.Net.Http.Json;
using MiSmart.API;
using MiSmart.Infrastructure.Constants;
using MiSmart.DAL.ViewModels;
using System.Net;

namespace MiSmart.Tests;


public sealed class BatteryModelsControllerTest : AuthorizedControllerTest<MiSmart.API.Startup>
{
    public BatteryModelsControllerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
    {
    }


    [Fact]
    public async Task GetListBatteryModelAndReturnListBatteryModel()
    {
        var client = CreateAuthorizedClient();
        var resp = await client.GetAsync("/batterymodels");

        ListResponseTest<SmallBatteryModelViewModel>? listResponse = await resp.Content.ReadFromJsonAsync<ListResponseTest<SmallBatteryModelViewModel>>();


        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.NotNull(listResponse);
        Assert.NotNull(listResponse?.Data);
        Assert.NotEmpty(listResponse?.Data);
    }
    [Fact]
    public async Task CreateBatteryModelWithRoleStaffAndReturnForbiddenMessage()
    {
        var user = GenerateStaffUser();
        var client = CreateAuthorizedClient(user);
        var resp = await client.PostAsJsonAsync("/batterymodels", new { Name = "Test model", ManufacturerName = "asfnasfgkjasng" }, JsonSerializerDefaultOptions.CamelOptions);
        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }
    [Fact]
    public async Task CreateBatteryModelWithRoleAdminAndReturnCreatedObject()
    {
        var client = CreateAuthorizedClient();

        var resp = await client.PostAsJsonAsync("/batterymodels", new
        {
            Name = "Test model",
            ManufacturerName = "asfnasfgkjasng"
        }, JsonSerializerDefaultOptions.CamelOptions);

        CreateResponseTest<Int32>? createResponse = await resp.Content.ReadFromJsonAsync<CreateResponseTest<Int32>>(JsonSerializerDefaultOptions.CamelOptions);

        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        Assert.NotNull(createResponse);
        Assert.NotNull(createResponse?.Data);
        Assert.True(createResponse?.Data?.ID > 0);
    }
    private readonly Int32 testingBatteryModelID = 1;
    [Fact]
    public async Task UpdateBatteryModelWithRoleStaffAndReturnForbiddenMessage()
    {
        var client = CreateAuthorizedClient(GenerateStaffUser());
        var resp = await client.PatchAsync($"/batterymodels/{testingBatteryModelID}", JsonContent.Create(new
        {
            Name = "update test"
        }, options: JsonSerializerDefaultOptions.CamelOptions));

        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }

    [Fact]
    public async Task UpdateBatteryModelWithRoleAdminAndReturnUpdatedMessage()
    {
        var client = CreateAuthorizedClient();

        var resp = await client.PatchAsync($"/batterymodels/{testingBatteryModelID}", JsonContent.Create(new
        {
            Name = "update test"
        }, options: JsonSerializerDefaultOptions.CamelOptions));

        UpdatedResponseTest? updatedResponseTest = await resp.Content.ReadFromJsonAsync<UpdatedResponseTest>(JsonSerializerDefaultOptions.CamelOptions);

        Assert.NotNull(updatedResponseTest);
        Assert.NotNull(updatedResponseTest?.Message);
    }
    [Fact]
    public async Task RemoveBatteryModelWithRoleStaffAndReturnForbiddenMessage()
    {
        var client = CreateAuthorizedClient(GenerateStaffUser());

        var resp = await client.DeleteAsync($"batterymodels/{testingBatteryModelID}");

        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }
}


using System.Net.Http.Json;
using MiSmart.API;
using MiSmart.Infrastructure.Constants;
using System.Net;
using MiSmart.DAL.ViewModels;

namespace MiSmart.Tests;


public sealed class BatteriesControllerTest : AuthorizedControllerTest<Startup>
{
    public BatteriesControllerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
    {
    }
    private readonly Int32 batteryID = 1;
    [Fact]
    public async Task PatchBatteryWithRoleAdminAndReturnUpdatedMessage()
    {
        var client = CreateAuthorizedClient();

        var resp = await client.PatchAsync($"/batteries/{batteryID}", JsonContent.Create(new
        {
            Name = "afasfmv,zxv_tst",
            BatteryModelID = 2
        }, options: JsonSerializerDefaultOptions.CamelOptions));

        UpdatedResponseTest? updatedResponse = await resp.Content.ReadFromJsonAsync<UpdatedResponseTest>(JsonSerializerDefaultOptions.CamelOptions);
        Assert.NotNull(updatedResponse);
        Assert.NotNull(updatedResponse?.Message);
    }
    [Fact]
    public async Task PatchBatteryWithRoleStaffAndReturnForbiddenMessage()
    {
        var client = CreateAuthorizedClient(GenerateStaffUser());

        var resp = await client.PatchAsync($"/batteries/{batteryID}", JsonContent.Create(new
        {
            Name = "afasfmv,zxv_tst",
            BatteryModelID = 2
        }, options: JsonSerializerDefaultOptions.CamelOptions));

        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }
    [Fact]
    public async Task CreateBatteryWithRoleAdminAndReturnCreatedObject()
    {
        var client = CreateAuthorizedClient();
        var resp = await client.PostAsJsonAsync("/batteries", new
        {
            Name = "afasfaf",
            ActualID = "12412412",
            BatteryModelID = 1,
        }, JsonSerializerDefaultOptions.CamelOptions);

        CreateResponseTest<Int32>? createResponse = await resp.Content.ReadFromJsonAsync<CreateResponseTest<Int32>>(JsonSerializerDefaultOptions.CamelOptions);



        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        Assert.NotNull(createResponse);
        Assert.NotNull(createResponse?.Data);
        Assert.True(createResponse?.Data?.ID > 0);
    }
    [Fact]
    public async Task CreateBatteryWithRoleStaffAndReturnForbiddenMessage()
    {
        var client = CreateAuthorizedClient(GenerateStaffUser());
        var resp = await client.PostAsJsonAsync("/batteries", new
        {
            Name = "afasfaf",
            ActualID = "12412412",
            BatteryModelID = 1,
        }, JsonSerializerDefaultOptions.CamelOptions);

        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }
    [Fact]
    public async Task GetListBatteryWithRoleAdminAndReturnListBattery()
    {
        var client = CreateAuthorizedClient();
        var resp = await client.GetAsync("/batteries?relation=Administrator&pageIndex=0&pageSize=5");

        ListResponseTest<BatteryViewModel>? listResponse = await resp.Content.ReadFromJsonAsync<ListResponseTest<BatteryViewModel>>(JsonSerializerDefaultOptions.CamelOptions);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.NotNull(listResponse);
        Assert.NotNull(listResponse?.Data);
        Assert.NotEmpty(listResponse?.Data);


    }
}
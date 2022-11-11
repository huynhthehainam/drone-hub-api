using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MiSmart.DAL.ViewModels;
using MiSmart.Infrastructure.Constants;

namespace MiSmart.Tests;
public class ListResponseTest<T> where T : class
{
    public List<T>? Data { get; set; }
    public Int32 TotalItems { get; set; }
}
public sealed class UpdatedResponseTest
{
    public String? Message { get; set; }
}

public class CreateResponseTestData<T> where T : struct
{
    public T ID { get; set; }
}
public class CreateResponseTest<T> where T : struct
{
    public CreateResponseTestData<T>? Data { get; set; }
}
public sealed class CustomersControllerTest : AuthorizedControllerTest<MiSmart.API.Startup>
{

    public CustomersControllerTest(CustomWebApplicationFactory<MiSmart.API.Startup> factory) : base(factory)
    {

    }
    private readonly Int32 testingCustomerID = 1;

    public static String[] Names = new String[] {
  "Tom", "Rich", "Barry",
  "Chris","Mary","Kate",
  "Mo","Dil","Eddy",
  "Pat","Peter","Matt",
  "Jo","Anne","Don",
  "Sales","Eng","Training",
  "Tommy","Team A","Team B",
  "Andy","Rachel","Les"
};
    private Random rand = new Random(DateTime.Now.Second);
    [Fact]
    public async Task GetListCustomersWithRoleAdminAndReturnListCustomers()
    {
        var client = CreateAuthorizedClient();


        // Act
        var resp = await client.GetAsync("/customers?pageIndex=0&pageSize=5");

        // Assert
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var contentStr = await resp.Content.ReadAsStringAsync();
        ListResponseTest<SmallCustomerViewModel>? listResponse = JsonSerializer.Deserialize<ListResponseTest<SmallCustomerViewModel>>(contentStr, JsonSerializerDefaultOptions.CamelOptions);
        Assert.NotNull(listResponse);
        Assert.NotEmpty(listResponse?.Data);
    }
    [Fact]
    public async Task CreateCustomerWithRoleAdminAndReturnCreatedObject()
    {
        var client = CreateAuthorizedClient();

        // Act
        int randIndex1 = rand.Next(0, Names.Length - 1);
        int randIndex2 = rand.Next(0, Names.Length - 1);
        var resp = await client.PostAsync("/customers", JsonContent.Create(new
        {
            Name = $"{Names[randIndex1]} {Names[randIndex2]}",
            Address = "Test address",
        }, options: JsonSerializerDefaultOptions.CamelOptions));

        // Assert
        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        CreateResponseTest<Int32>? createResponse = await resp.Content.ReadFromJsonAsync<CreateResponseTest<Int32>>(JsonSerializerDefaultOptions.CamelOptions);

        Assert.NotNull(createResponse);
        Assert.True(createResponse?.Data?.ID > 0);
    }

    [Fact]
    public async Task AssignUserToCustomerReturnCreatedObject()
    {

        var client = CreateAuthorizedClient();

        // Act



        var resp = await client.PostAsJsonAsync($"/customers/{testingCustomerID}/assignuser", new { UserUUID = Guid.NewGuid() }, JsonSerializerDefaultOptions.CamelOptions);

        CreateResponseTest<Int64>? createResponse = await resp.Content.ReadFromJsonAsync<CreateResponseTest<Int64>>(JsonSerializerDefaultOptions.CamelOptions);

        // Assert
        Assert.NotNull(createResponse);
        Assert.NotEqual(0, createResponse?.Data?.ID);
    }

    [Fact]
    public async Task UpdateCustomerAndReturnUpdatedMessage()
    {
        var client = CreateAuthorizedClient();

        int randIndex1 = rand.Next(0, Names.Length - 1);
        var resp = await client.PatchAsync($"customers/{testingCustomerID}", JsonContent.Create(new { Name = $"updated_${Names[randIndex1]}" }, options: JsonSerializerDefaultOptions.CamelOptions));
        UpdatedResponseTest? updatedResponse = await resp.Content.ReadFromJsonAsync<UpdatedResponseTest>(JsonSerializerDefaultOptions.CamelOptions);
        Assert.NotNull(updatedResponse);
        Assert.NotNull(updatedResponse?.Message);
    }
    [Fact]
    public async Task DeleteCustomerWithRoleStaffAndReturnFailedStatus()
    {
        var client = CreateAuthorizedClient(
                 new Infrastructure.ViewModels.UserCacheViewModel
                 {
                     ID = 1,
                     IsAdmin = false,
                     IsActive = true,
                     RoleID = 2,
                     Email = "test@mismart.ai",
                     Username = "test@mismart.ai",
                 });

        var resp = await client.DeleteAsync($"customers/{testingCustomerID}");

        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }
    [Fact]
    public async Task AddDeviceToCustomerAndReturnCreatedObject()
    {
        var client = CreateAuthorizedClient();

        Int32 deviceModelID = 1;
        String name = "Device test";

        var resp = await client.PostAsJsonAsync($"/customers/{testingCustomerID}/devices", new
        {
            Name = name,
            DeviceModelID = deviceModelID
        }, JsonSerializerDefaultOptions.CamelOptions);
        var data = await resp.Content.ReadAsStringAsync();

        CreateResponseTest<Int32>? createResponse = await resp.Content.ReadFromJsonAsync<CreateResponseTest<Int32>>(JsonSerializerDefaultOptions.CamelOptions);
        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        Assert.NotNull(createResponse);
        Assert.NotNull(createResponse?.Data);
        Assert.True(createResponse?.Data?.ID > 0);
    }
    [Fact]
    public async Task GetListDeviceAndReturnListDevice()
    {
        var client = CreateAuthorizedClient();

        var resp = await client.GetAsync($"/customers/{testingCustomerID}/devices");

        ListResponseTest<SmallDeviceViewModel>? listResponse = await resp.Content.ReadFromJsonAsync<ListResponseTest<SmallDeviceViewModel>>(JsonSerializerDefaultOptions.CamelOptions);

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.NotNull(listResponse);
        Assert.NotNull(listResponse?.Data);
        Assert.NotEmpty(listResponse?.Data);

    }
}
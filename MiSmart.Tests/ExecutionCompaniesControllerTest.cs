

using System.Net;
using System.Net.Http.Json;
using MiSmart.API;
using MiSmart.Infrastructure.Constants;

namespace MiSmart.Tests;
[Collection("Sequential")]
public sealed class ExecutionCompaniesControllerTest : AuthorizedControllerTest<MiSmart.API.Startup>
{
    public ExecutionCompaniesControllerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
    {
    }
    public Object GenerateCreateExecutionCompanyCommand()
    {
        return new
        {
            Name = "sfasftest",
            Address = "asfasfaf"
        };
    }
    private readonly Int32 testingExecutionCompanyID = 1;
    [Fact]
    public async Task CreateExecutionCompanyWithRoleAdminAndReturnCreatedObject()
    {
        var client = CreateAuthorizedClient();
        var resp = await client.PostAsJsonAsync("/executioncompanies/", GenerateCreateExecutionCompanyCommand(), JsonSerializerDefaultOptions.CamelOptions);

        CreateResponseTest<Int32>? createResponse = await resp.Content.ReadFromJsonAsync<CreateResponseTest<Int32>>(JsonSerializerDefaultOptions.CamelOptions);

        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        Assert.NotNull(createResponse);
        Assert.True(createResponse?.Data?.ID > 0);
    }
    [Fact]
    public async Task CreateExecutionCompanyWithRoleStaffAndReturnForbiddenMessage()
    {
        var client = CreateAuthorizedClient(GenerateStaffUser());
        var resp = await client.PostAsJsonAsync("/executioncompanies/", GenerateCreateExecutionCompanyCommand(), JsonSerializerDefaultOptions.CamelOptions);

        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }
    [Fact]
    public async Task PatchExecutionCompanyWithRoleAdminAndReturnUpdatedMessage()
    {
        var client = CreateAuthorizedClient();

        var resp = await client.PatchAsync($"/executioncompanies/{testingExecutionCompanyID}", JsonContent.Create(new
        {
            Name = "Updtead"
        }, options: JsonSerializerDefaultOptions.CamelOptions));

        UpdatedResponseTest? updatedResponse = await resp.Content.ReadFromJsonAsync<UpdatedResponseTest>(JsonSerializerDefaultOptions.CamelOptions);

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.NotNull(updatedResponse);
        Assert.NotNull(updatedResponse?.Message);
    }
    [Fact]
    public async Task PatchExecutionCompanyWithRoleStaffAndReturnUpdatedMessage()
    {
        var client = CreateAuthorizedClient(GenerateStaffUser());

        var resp = await client.PatchAsync($"/executioncompanies/{testingExecutionCompanyID}", JsonContent.Create(new
        {
            Name = "Updtead"
        }, options: JsonSerializerDefaultOptions.CamelOptions));

        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);

    }
    [Fact]
    public async Task AssignUserWithRoleAdminAndReturnCreatedObject()
    {
        var client = CreateAuthorizedClient();
        var resp = await client.PostAsJsonAsync($"/executioncompanies/{testingExecutionCompanyID}/assignuser", new
        {
            UserUUID = Guid.NewGuid(),
            Type = "Owner"
        }, JsonSerializerDefaultOptions.CamelOptions);

        var data = await resp.Content.ReadAsStringAsync();
        Console.WriteLine($"data {data}");
        CreateResponseTest<Int64>? createResponse = await resp.Content.ReadFromJsonAsync<CreateResponseTest<Int64>>(JsonSerializerDefaultOptions.CamelOptions);

        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        Assert.NotNull(createResponse);
        Assert.True(createResponse?.Data?.ID > 0);
    }
    [Fact]
    public async Task AssignUserWithRoleStaffAndReturnForbiddenMessage()
    {
        var client = CreateAuthorizedClient(GenerateStaffUser());
        var resp = await client.PostAsJsonAsync($"/executioncompanies/{testingExecutionCompanyID}/assignuser", new
        {
            UserUUID = Guid.NewGuid(),
            Type = "Owner"
        }, JsonSerializerDefaultOptions.CamelOptions);

        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }
}
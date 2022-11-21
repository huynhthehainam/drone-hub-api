using System.Net;
using System.Net.Http.Json;
using MiSmart.API;
using MiSmart.Infrastructure.Constants;

namespace MiSmart.Tests;
[Collection("Sequential")]
public sealed class DevicesControllerTest : AuthorizedControllerTest<MiSmart.API.Startup>
{
    public DevicesControllerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
    {
    }

    public Object CreateAssignExecutionCompanyCommand()
    {
        return new
        {
            ExecutionCompanyID = 1
        };
    }
    private readonly Int32 testingDeviceID = 2;
    [Fact]
    public async Task AssignExecutionCompanyFromCustomerUserAndReturnUpdatedMessage()
    {

        var client = CreateAuthorizedClient();
        var resp = await client.PostAsJsonAsync($"/devices/{testingDeviceID}/assignexecutioncompany", CreateAssignExecutionCompanyCommand(), JsonSerializerDefaultOptions.CamelOptions);


        UpdatedResponseTest? updatedResponse = await resp.Content.ReadFromJsonAsync<UpdatedResponseTest>(JsonSerializerDefaultOptions.CamelOptions);
        var data = await resp.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.NotNull(updatedResponse);
        Assert.NotNull(updatedResponse?.Message);
    }
    [Fact]
    public async Task AssignExecutionCompanyFromAnonymousAndReturnForbiddenMessage()
    {

        var client = CreateAuthorizedClient(GenerateStaffUser());
        var resp = await client.PostAsJsonAsync($"/devices/{testingDeviceID}/assignexecutioncompany", CreateAssignExecutionCompanyCommand(), JsonSerializerDefaultOptions.CamelOptions);
        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }
}

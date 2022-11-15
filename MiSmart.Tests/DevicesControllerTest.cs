using MiSmart.API;

namespace MiSmart.Tests;
public sealed class DevicesControllerTest : AuthorizedControllerTest<MiSmart.API.Startup>
{
    public DevicesControllerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
    {
    }
    
}

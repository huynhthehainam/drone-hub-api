
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiSmart.Infrastructure.Constants;

namespace MiSmart.Tests;
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
       ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
       : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] {
                new Claim(Keys.JWTAuthKey, 1.ToString()),
                new Claim(Keys.JWTAdminKey, true.ToString()),
                new Claim(Keys.JWTRoleKey,1.ToString()),
                new Claim(Keys.JWTUUIDKey,"c54e8db9-65d5-445b-86c9-0130a848c185" ),
                new Claim(Keys.JWTUserTypeKey, "User"),
             };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        var result = AuthenticateResult.Success(ticket);
        Console.WriteLine("init auth handler");
        return Task.FromResult(result);
    }
}
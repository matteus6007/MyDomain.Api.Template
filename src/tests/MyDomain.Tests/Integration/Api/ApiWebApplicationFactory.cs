using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MyDomain.Tests.Integration.Api;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string Audience = "mydomain-api";
    private readonly string _issuer;
    private readonly JsonWebKey _jwk;

    public ApiWebApplicationFactory()
    {
        _issuer = "http://localhost:8081/identity/";

        //-:cnd:noEmit
#if (!DEBUG)
        _issuer = "http://wiremock:8080/identity/";
#endif
        //+:cnd:noEmit

        _jwk = JsonWebKey.Create(File.ReadAllText("./Integration/Api/Resources/jwk.json"));
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "test");
        Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "test");

        // disable health check UI
        Environment.SetEnvironmentVariable("HealthChecksUI_Enabled", "false");
        Environment.SetEnvironmentVariable("MyDomain__Identity__Issuer", _issuer);

        builder.ConfigureAppConfiguration((hostingContext, config) => config.AddEnvironmentVariables());
    }

    public string GenerateTokenFor(string scope)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("scope", scope)
            }),
            Expires = DateTime.UtcNow.AddMinutes(10),
            Issuer = _issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(_jwk, SecurityAlgorithms.RsaSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
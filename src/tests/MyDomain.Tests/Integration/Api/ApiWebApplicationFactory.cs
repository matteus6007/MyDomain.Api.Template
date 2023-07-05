using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace MyDomain.Tests.Integration.Api;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "test");
        Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "test");

        // disable health check UI
        Environment.SetEnvironmentVariable("HealthChecksUI_Enabled", "false");

        builder.ConfigureAppConfiguration((hostingContext, config) => config.AddEnvironmentVariables());
    }
}
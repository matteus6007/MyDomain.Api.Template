using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

using MyDomain.Api;
using MyDomain.Api.Middleware;
using MyDomain.Application;
using MyDomain.Infrastructure;
using MyDomain.Infrastructure.Messaging;

using Serilog;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddPresentation();
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddMessaging(builder.Configuration);
    builder.Services
        .ConfigureHealthChecks(builder.Configuration)
        .AddInfrastructureHealthChecks(builder.Configuration);
}

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<BuildVersionResponseMiddleware>();
app.UseExceptionHandler("/error");
app.UseRouting();
app.UseHealthChecks("/healthcheck/tests", new HealthCheckOptions 
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapControllers();
app.MapHealthChecksUI();
app.Run();

public partial class Program { }

using System.Reflection;

using FluentValidation;
using FluentValidation.AspNetCore;

using MyDomain.Api.Common.Mapping;
using MyDomain.Api.Options;

namespace MyDomain.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddMappings();
        services.AddProblemDetails();
        services.ConfigureOptions<AssemblyOptionsProvider>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        var defaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new() { Title = "MyDomain API", Version = "v1" });

            var filePath = Path.Combine(
                AppContext.BaseDirectory,
                $"{typeof(Program).Assembly.GetName().Name}.xml");

            options.IncludeXmlComments(filePath);
        });
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = defaultApiVersion;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });
        services.AddVersionedApiExplorer(options =>
        {
            options.SubstituteApiVersionInUrl = true;
            options.GroupNameFormat = "'v'V";
            options.DefaultApiVersion = defaultApiVersion;
            options.AssumeDefaultVersionWhenUnspecified = true;
        });

        return services;
    }
}
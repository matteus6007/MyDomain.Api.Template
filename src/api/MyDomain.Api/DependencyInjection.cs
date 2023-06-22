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
        var defaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1,0);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddApiVersioning(opt => 
        {
            opt.DefaultApiVersion = defaultApiVersion;
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
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
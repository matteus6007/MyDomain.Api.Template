using MyDomain.Api.Common.Mapping;

namespace MyDomain.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddMappings();

        return services;
    }
}
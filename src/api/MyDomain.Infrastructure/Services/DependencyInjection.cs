using Microsoft.Extensions.DependencyInjection;

using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Infrastructure.Persistence;

namespace MyDomain.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IMyAggregateRepository, InMemoryMyAggregateRepository>();

            return services;
        }
    }
}
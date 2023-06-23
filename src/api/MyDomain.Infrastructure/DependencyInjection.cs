using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Infrastructure.Persistence;
using MyDomain.Infrastructure.Persistence.Options;

namespace MyDomain.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            MyAggregateIdTypeHandler.AddTypeHandlers();

            services.Configure<DatabaseOptions>(setting => configuration.GetSection("MyDomain:Database"));
            services.AddSingleton<IMyAggregateRepository, InMemoryMyAggregateRepository>();
            //services.AddScoped<IMyAggregateRepository, MyAggregateRepository>();

            return services;
        }
    }
}
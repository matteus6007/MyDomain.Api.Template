using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain.Models;
using MyDomain.Infrastructure.Persistence.Options;
using MyDomain.Infrastructure.Persistence.QueryExecutors;
using MyDomain.Infrastructure.Persistence.Repositories;

namespace MyDomain.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            MyAggregateIdTypeHandler.AddTypeHandlers();

            services.Configure<DatabaseOptions>(opts => configuration.GetSection("MyDomain:Database").Bind(opts));
            //services.AddSingleton<IMyAggregateRepository, InMemoryMyAggregateRepository>();
            services.AddScoped<IMyAggregateRepository, MyAggregateRepository>();
            services.AddScoped<IQueryExecutor<MyDomainReadModel, Guid>, MyDomainQueryExecutor>();

            return services;
        }
    }
}
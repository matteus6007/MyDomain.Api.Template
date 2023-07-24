using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain.Models;
using MyDomain.Domain.MyDomainAggregate;
using MyDomain.Domain.MyDomainAggregate.ValueObjects;
using MyDomain.Infrastructure.Persistence;
using MyDomain.Infrastructure.Persistence.Options;
using MyDomain.Infrastructure.Persistence.QueryExecutors;
using MyDomain.Infrastructure.Persistence.Repositories;

namespace MyDomain.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        MyDomainIdTypeHandler.AddTypeHandlers();

        services.Configure<DatabaseOptions>(opts => configuration.GetSection(DatabaseOptions.SectionName).Bind(opts));
        // services.AddScoped<IReadRepository<MyAggregate, MyAggregateId>, InMemoryMyAggregateRepository>();
        // services.AddScoped<IWriteRepository<MyAggregate, MyAggregateId>, InMemoryMyAggregateRepository>();
        services.AddScoped<IReadRepository<MyDomainAggregate, MyDomainId>, MyDomainAggregateRepository>();
        services.AddScoped<IWriteRepository<MyDomainAggregate, MyDomainId>, MyDomainAggregateRepository>();
        services.AddScoped<IAggregatePersistenceService<MyDomainAggregate, MyDomainId>, AggregatePersistenceService<MyDomainAggregate, MyDomainId>>();
        services.AddScoped<IQueryExecutor<MyDomainReadModel, Guid>, MyDomainQueryExecutor>();

        return services;
    }
}
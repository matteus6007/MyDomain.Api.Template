using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain.Models;
using MyDomain.Domain.MyAggregate;
using MyDomain.Domain.MyAggregate.ValueObjects;
using MyDomain.Infrastructure.Persistence;
using MyDomain.Infrastructure.Persistence.Options;
using MyDomain.Infrastructure.Persistence.QueryExecutors;
using MyDomain.Infrastructure.Persistence.Repositories;

namespace MyDomain.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        MyAggregateIdTypeHandler.AddTypeHandlers();

        services.Configure<DatabaseOptions>(opts => configuration.GetSection(DatabaseOptions.SectionName).Bind(opts));
        // services.AddScoped<IReadRepository<MyAggregate, MyAggregateId>, InMemoryMyAggregateRepository>();
        // services.AddScoped<IWriteRepository<MyAggregate, MyAggregateId>, InMemoryMyAggregateRepository>();
        services.AddScoped<IReadRepository<MyAggregate, MyAggregateId>, MyAggregateRepository>();
        services.AddScoped<IWriteRepository<MyAggregate, MyAggregateId>, MyAggregateRepository>();
        services.AddScoped<IAggregatePersistenceService<MyAggregate, MyAggregateId>, AggregatePersistenceService<MyAggregate, MyAggregateId>>();
        services.AddScoped<IQueryExecutor<MyDomainReadModel, Guid>, MyDomainQueryExecutor>();

        return services;
    }
}
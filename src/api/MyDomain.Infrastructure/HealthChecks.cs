using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MyDomain.Infrastructure.Persistence.Options;

namespace MyDomain.Infrastructure;

public static class HealthChecks
{
    public static IHealthChecksBuilder AddInfrastructureHealthChecks(this IHealthChecksBuilder healthChecksBuilder, IConfiguration configuration)
    {
        var databaseOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>();

        if (databaseOptions == null)
        {
            return healthChecksBuilder;
        }

        List<string> readDbTags = new() { databaseOptions.Server };

        if (!string.IsNullOrEmpty(databaseOptions.ReplicaServer))
        {
            readDbTags.Add(databaseOptions.ReplicaServer);
        }

        healthChecksBuilder.AddMySql(
            databaseOptions.ReadConnectionString(),
            name: "Read MySql Database",
            tags: readDbTags);
        healthChecksBuilder.AddMySql(
            databaseOptions.WriteConnectionString(),
            name: "Write MySql Database",
            tags: new[] { databaseOptions.Server });

        return healthChecksBuilder;
    }    
}
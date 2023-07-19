using System.Data;

using Dapper;

using Microsoft.Extensions.Options;

using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain.Models;
using MyDomain.Infrastructure.Persistence.Options;

using MySql.Data.MySqlClient;

namespace MyDomain.Infrastructure.Persistence.QueryExecutors;

public class MyDomainQueryExecutor : IQueryExecutor<MyDomainReadModel, Guid>
{
    private readonly string _readConnectionString;

    public MyDomainQueryExecutor(IOptionsSnapshot<DatabaseOptions> options)
    {
        _readConnectionString = options.Value.ReadConnectionString();
    }

    public async Task<MyDomainReadModel?> GetByIdAsync(Guid id)
    {
        using var connection = new MySqlConnection(_readConnectionString);

        const string sql = @"SELECT Id,Name,Description,CreatedOn,UpdatedOn
                            FROM MyAggregates
                            WHERE Id = @Id;";

        var model = await connection.QuerySingleOrDefaultAsync<MyDomainReadModel>(
            sql,
            new { id = id.ToString() },
            commandType: CommandType.Text);

        return model;
    }
}
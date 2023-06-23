using System.Data;

using Dapper;

using Microsoft.Extensions.Options;

using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain.MyAggregate;
using MyDomain.Domain.MyAggregate.ValueObjects;
using MyDomain.Infrastructure.Persistence.Options;

using MySql.Data.MySqlClient;

namespace MyDomain.Infrastructure.Persistence;

public class MyAggregateRepository : IMyAggregateRepository
{
    private readonly string _readConnectionString;
    private readonly string _writeConnectionString;

    public MyAggregateRepository(IOptionsSnapshot<DatabaseOptions> options)
    {
        _readConnectionString = options.Value.ReadConnectionString();
        _writeConnectionString = options.Value.WriteConnectionString();
    }

    public async Task<MyAggregate?> GetByIdAsync(MyAggregateId id)
    {
        using var connection = new MySqlConnection(_readConnectionString);
        
        const string sql = @"SELECT Id,Version,Name,Description,CreatedOn,UpdatedOn
                            FROM MyAggregates
                            WHERE Id = @Id;";

        var aggregate = await connection.QuerySingleOrDefaultAsync<MyAggregate>(
            sql,
            new { id = id.Value.ToString() },
            commandType: CommandType.Text);

        return aggregate;
    }    

    public Task AddAsync(MyAggregate data)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(MyAggregate data)
    {
        throw new NotImplementedException();
    }
}
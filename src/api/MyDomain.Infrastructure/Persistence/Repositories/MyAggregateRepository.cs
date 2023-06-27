using System.Data;

using Dapper;

using Microsoft.Extensions.Options;

using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain.MyAggregate;
using MyDomain.Domain.MyAggregate.ValueObjects;
using MyDomain.Infrastructure.Persistence.Options;

using MySql.Data.MySqlClient;

namespace MyDomain.Infrastructure.Persistence.Repositories;

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

    public async Task AddAsync(MyAggregate data)
    {
        using var connection = new MySqlConnection(_writeConnectionString);
        await connection.OpenAsync();

        using var transaction = await connection.BeginTransactionAsync();
        var parameters = new
        {
            Id = data.Id.Value.ToString(),
            data.Version,
            data.Name,
            data.Description,
            data.CreatedOn,
            data.UpdatedOn
        };

        const string sql = @"INSERT INTO MyAggregates (Id,Version,Name,Description,CreatedOn,UpdatedOn)
                            VALUES (@Id,@Version,@Name,@Description,@CreatedOn,@UpdatedOn);";

        await connection.ExecuteAsync(
            sql,
            parameters,
            transaction,
            commandType: CommandType.Text);

        transaction.Commit();
    }

    public async Task UpdateAsync(MyAggregate data)
    {
        using var connection = new MySqlConnection(_writeConnectionString);
        await connection.OpenAsync();

        using var transaction = await connection.BeginTransactionAsync();
        var parameters = new
        {
            Id = data.Id.Value.ToString(),
            data.Version,
            data.Name,
            data.Description,
            data.CreatedOn,
            data.UpdatedOn
        };

        // TODO: Add version check
        const string sql = @"UPDATE MyAggregates SET Version = @Version, Name = @Name, Description = @Description, CreatedOn = @CreatedOn, UpdatedOn = @UpdatedOn
                            WHERE Id = @Id;";

        var recordsUpdated = await connection.ExecuteAsync(
            sql,
            parameters,
            transaction,
            commandType: CommandType.Text);

        transaction.Commit();
    }
}
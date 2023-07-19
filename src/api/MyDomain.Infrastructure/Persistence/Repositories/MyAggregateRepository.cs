using System.Data;

using Dapper;

using ErrorOr;

using Microsoft.Extensions.Options;

using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain;
using MyDomain.Domain.Common.Interfaces;
using MyDomain.Domain.MyAggregate;
using MyDomain.Domain.MyAggregate.ValueObjects;
using MyDomain.Infrastructure.Persistence.Options;

using MySql.Data.MySqlClient;

namespace MyDomain.Infrastructure.Persistence.Repositories;

public class MyAggregateRepository :
    IReadRepository<MyAggregate, MyAggregateId>,
    IWriteRepository<MyAggregate, MyAggregateId>
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

        var aggregateState = await connection.QuerySingleOrDefaultAsync<MyAggregateState>(
            sql,
            new { id = id.Value.ToString() },
            commandType: CommandType.Text);

        return aggregateState == null ? null : new MyAggregate(aggregateState);
    }

    public async Task<ErrorOr<Created>> AddAsync(MyAggregate data)
    {
        using var connection = new MySqlConnection(_writeConnectionString);
        await connection.OpenAsync();

        using var transaction = await connection.BeginTransactionAsync();
        var parameters = new
        {
            Id = data.Id.Value,
            data.Version,
            data.State.Name,
            data.State.Description,
            data.State.CreatedOn,
            data.State.UpdatedOn
        };

        const string sql = @"INSERT INTO MyAggregates (Id,Version,Name,Description,CreatedOn,UpdatedOn)
                            VALUES (@Id,@Version,@Name,@Description,@CreatedOn,@UpdatedOn);";

        await connection.ExecuteAsync(
            sql,
            parameters,
            transaction,
            commandType: CommandType.Text);

        transaction.Commit();

        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(MyAggregate data)
    {
        using var connection = new MySqlConnection(_writeConnectionString);
        await connection.OpenAsync();

        using var transaction = await connection.BeginTransactionAsync();
        var parameters = new
        {
            Id = data.Id.Value,
            data.Version,
            data.PreviousVersion,
            data.State.Name,
            data.State.Description,
            data.State.CreatedOn,
            data.State.UpdatedOn
        };

        // TODO: Add version check
        const string sql = @"UPDATE MyAggregates SET Version = @Version, Name = @Name, Description = @Description, CreatedOn = @CreatedOn, UpdatedOn = @UpdatedOn
                            WHERE Id = @Id AND Version = @PreviousVersion;";

        var recordsUpdated = await connection.ExecuteAsync(
            sql,
            parameters,
            transaction,
            commandType: CommandType.Text);

        if (recordsUpdated == 0)
        {
            var currentVersion = await GetCurrentVersionAsync(data.Id.Value, transaction);

            if (currentVersion == 0)
            {
                return Error.NotFound();
            }

            return Error.Conflict(description: $"Version is out of date. Expected: {data.PreviousVersion}. Required: {currentVersion}");
        }

        transaction.Commit();

        return Result.Updated;
    }

    private static async Task<long> GetCurrentVersionAsync(Guid id, MySqlTransaction transaction)
    {
        const string sql = @"SELECT Version
                            FROM MyAggregates
                            WHERE Id = @Id;";

        var currentVersion = await transaction.Connection.ExecuteScalarAsync<long>(
            sql,
            new { id },
            transaction,
            commandType: CommandType.Text);

        return currentVersion;
    }
}
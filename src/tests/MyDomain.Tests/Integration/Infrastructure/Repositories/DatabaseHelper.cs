using System.Data;

using Dapper;

using MyDomain.Infrastructure.Persistence.Options;

using MySql.Data.MySqlClient;

namespace MyDomain.Tests.Integration.Repositories;

public class DatabaseHelper<TId, TRecord>
{
    private readonly string _tableName;
    private readonly string _idColumnName;
    private readonly string _readConnectionString;
    private readonly string _writeConnectionString;
    public readonly DatabaseOptions Options;

    public DatabaseHelper(
        string tableName,
        string idColumnName)
    {
        _tableName = tableName;
        _idColumnName = idColumnName;

        Options = new DatabaseOptions 
        {
            Server = "localhost",
            Port = 30000,
            Schema = "MyDomainSchema",
            User = "root",
            Password = "password123"
        };

#if (!DEBUG)
            Options.Server = "local-db";
            Options.Port = 3306;
#endif

        _readConnectionString = Options.ReadConnectionString();
        _writeConnectionString = Options.WriteConnectionString();
    }

    public async Task<T> GetRecordAsync<T>(TId id)
    {
        using (var connection = new MySqlConnection(_readConnectionString))
        {
            var sql = $@"SELECT * FROM {_tableName} WHERE {_idColumnName} = @Id LIMIT 1";

            return await connection.QueryFirstOrDefaultAsync<T>(sql, new { id });
        }
    }

    public async Task AddRecordAsync(TRecord record)
    {
        if (record == null)
        {
            return;
        }

        using (var connection = new MySqlConnection(_writeConnectionString))
        {
            var properties = record.GetType().GetProperties().Select(x => x.Name).ToList();

            var sql = $@"INSERT INTO {_tableName} 
                        ({string.Join(",", properties.Select(x => $"`{x}`"))})
                        VALUES ({string.Join(",", properties.Select(x => $"@{x}"))})";

            await connection.ExecuteAsync(sql, ToDynamicParameters(record), commandType: CommandType.Text);
        }
    }

    private static DynamicParameters? ToDynamicParameters<T>(T record)
    {
        if (record == null)
        {
            return null;
        }

        var recordType = record.GetType();
        var properties = recordType.GetProperties();

        var nonEnumPropertiesByName = properties.Where(pi => !pi.PropertyType.IsEnum).Select(x => new KeyValuePair<string, object>(x.Name, x.GetValue(record)));
        var enumProperties = properties.Where(pi => pi.PropertyType.IsEnum);
        var valuesByPropertyName = new List<KeyValuePair<string, object>>(nonEnumPropertiesByName);
        valuesByPropertyName.AddRange(enumProperties.Select(ep => new KeyValuePair<string, object>(ep.Name, ep.GetValue(record)?.ToString())));

        return new DynamicParameters(valuesByPropertyName);
    }   
}
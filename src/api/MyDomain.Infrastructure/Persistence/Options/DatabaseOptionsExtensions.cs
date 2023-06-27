namespace MyDomain.Infrastructure.Persistence.Options;

public static class DatabaseOptionsExtensions
{
    public static string ReadConnectionString(this DatabaseOptions options)
    {
        var port = options.Port ?? 3306;

        if (string.IsNullOrEmpty(options.ReplicaServer))
        {
            return $"Server={options.Server};Port={port};Database={options.Schema};Uid={options.User};Pwd={options.Password};Allow User Variables=True";
        }

        var server = $"(address={options.Server}:{port}, priority=60),(address={options.ReplicaServer}:{port}, priority=100)";

        return $"Server={server};Database={options.Schema};Uid={options.User};Pwd={options.Password};Allow User Variables=True";
    }

    public static string WriteConnectionString(this DatabaseOptions options)
    {
        return $"Server={options.Server};Port={options.Port ?? 3306};Database={options.Schema};Uid={options.User};Pwd={options.Password};Allow User Variables=True";
    }
}
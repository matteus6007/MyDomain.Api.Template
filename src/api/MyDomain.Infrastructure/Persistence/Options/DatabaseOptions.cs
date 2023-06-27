namespace MyDomain.Infrastructure.Persistence.Options;

public class DatabaseOptions
{
    public required string Server { get; set; }
    public string ReplicaServer { get; set; } = "";
    public int? Port { get; set; }
    public required string Schema { get; set; }
    public required string User { get; set; }
    public required string Password { get; set; }
}

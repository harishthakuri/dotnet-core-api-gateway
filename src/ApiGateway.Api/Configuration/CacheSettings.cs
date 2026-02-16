namespace ApiGateway.Api.Configuration;

public class CacheSettings
{
    public string Provider { get; set; } = "InMemory"; // InMemory or Redis
    public RedisSettings Redis { get; set; } = new();
}

public class RedisSettings
{
    public string ConnectionString { get; set; } = "localhost:6379";
    public string InstanceName { get; set; } = "ApiGateway:";
}

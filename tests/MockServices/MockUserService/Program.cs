using Consul;

var builder = WebApplication.CreateBuilder(args);

// Add Consul client
builder.Services.AddSingleton<IConsulClient>(sp =>
{
    var consulHost = builder.Configuration["CONSUL_HOST"] ?? "localhost";
    var consulPort = int.Parse(builder.Configuration["CONSUL_PORT"] ?? "8500");
    
    return new ConsulClient(config =>
    {
        config.Address = new Uri($"http://{consulHost}:{consulPort}");
    });
});

var app = builder.Build();

// Mock user data
var users = new List<User>
{
    new User(1, "John Doe", "john.doe@example.com"),
    new User(2, "Jane Smith", "jane.smith@example.com"),
    new User(3, "Bob Johnson", "bob.johnson@example.com")
};

// Get all users
app.MapGet("/users", () => Results.Ok(users));

// Get user by ID
app.MapGet("/users/{id:int}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    return user is not null ? Results.Ok(user) : Results.NotFound(new { message = "User not found" });
});

// Create user
app.MapPost("/users", (User user) =>
{
    var newUser = user with { Id = users.Max(u => u.Id) + 1 };
    users.Add(newUser);
    return Results.Created($"/users/{newUser.Id}", newUser);
});

// Health check
app.MapGet("/health", () => Results.Ok(new { service = "MockUserService", status = "Healthy" }));

// Register with Consul on startup
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
var consulClient = app.Services.GetRequiredService<IConsulClient>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

lifetime.ApplicationStarted.Register(() =>
{
    var serviceName = builder.Configuration["SERVICE_NAME"] ?? "user-service";
    var servicePort = int.Parse(builder.Configuration["SERVICE_PORT"] ?? "5001");
    var serviceId = $"{serviceName}-{Environment.MachineName}-{servicePort}";
    
    var registration = new AgentServiceRegistration
    {
        ID = serviceId,
        Name = serviceName,
        Address = "localhost",
        Port = servicePort,
        Tags = new[] { "api", "users", "v1" },
        Check = new AgentServiceCheck
        {
            HTTP = $"http://localhost:{servicePort}/health",
            Interval = TimeSpan.FromSeconds(10),
            Timeout = TimeSpan.FromSeconds(5),
            DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
        }
    };
    
    logger.LogInformation("Registering service {ServiceId} with Consul", serviceId);
    
    try
    {
        consulClient.Agent.ServiceDeregister(serviceId).Wait();
        consulClient.Agent.ServiceRegister(registration).Wait();
        logger.LogInformation("Service registered successfully with Consul");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to register with Consul");
    }
});

lifetime.ApplicationStopping.Register(() =>
{
    var serviceName = builder.Configuration["SERVICE_NAME"] ?? "user-service";
    var servicePort = int.Parse(builder.Configuration["SERVICE_PORT"] ?? "5001");
    var serviceId = $"{serviceName}-{Environment.MachineName}-{servicePort}";
    
    logger.LogInformation("Deregistering service {ServiceId} from Consul", serviceId);
    
    try
    {
        consulClient.Agent.ServiceDeregister(serviceId).Wait();
        logger.LogInformation("Service deregistered successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to deregister from Consul");
    }
});

app.Run();

record User(int Id, string Name, string Email);

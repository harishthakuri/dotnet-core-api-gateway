// using Consul;
// using Microsoft.AspNetCore.Hosting.Server;
// using Microsoft.AspNetCore.Hosting.Server.Features;
//
// namespace ApiGateway.Api.Extensions;
//
// public static class ConsulServiceRegistration
// {
//     public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IConfiguration configuration)
//     {
//         var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
//         var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
//         var logger = app.ApplicationServices.GetRequiredService<ILogger<IConsulClient>>();
//
//         // Get server addresses
//         var server = app.ApplicationServices.GetRequiredService<IServer>();
//         var addressFeature = server.Features.Get<IServerAddressesFeature>();
//         
//         if (addressFeature == null || !addressFeature.Addresses.Any())
//         {
//             logger.LogWarning("No server addresses found for Consul registration");
//             return app;
//         }
//
//         var address = addressFeature.Addresses.First();
//         var uri = new Uri(address);
//
//         var serviceName = configuration["ServiceName"] ?? "api-gateway";
//         var serviceId = $"{serviceName}-{Environment.MachineName}-{uri.Port}";
//
//         var registration = new AgentServiceRegistration
//         {
//             ID = serviceId,
//             Name = serviceName,
//             Address = uri.Host == "+" || uri.Host == "*" ? "localhost" : uri.Host,
//             Port = uri.Port,
//             Tags = new[] { "api", "gateway", "ocelot" },
//             Check = new AgentServiceCheck
//             {
//                 HTTP = $"{uri.Scheme}://{(uri.Host == "+" || uri.Host == "*" ? "localhost" : uri.Host)}:{uri.Port}/health",
//                 Interval = TimeSpan.FromSeconds(10),
//                 Timeout = TimeSpan.FromSeconds(5),
//                 DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
//             }
//         };
//
//         logger.LogInformation("Registering service {ServiceId} with Consul at {Address}:{Port}", 
//             serviceId, registration.Address, registration.Port);
//
//         try
//         {
//             consulClient.Agent.ServiceDeregister(serviceId).Wait();
//             consulClient.Agent.ServiceRegister(registration).Wait();
//             logger.LogInformation("Service registered successfully with Consul");
//         }
//         catch (Exception ex)
//         {
//             logger.LogError(ex, "Failed to register service with Consul");
//         }
//
//         lifetime.ApplicationStopping.Register(() =>
//         {
//             logger.LogInformation("Deregistering service {ServiceId} from Consul", serviceId);
//             try
//             {
//                 consulClient.Agent.ServiceDeregister(serviceId).Wait();
//                 logger.LogInformation("Service deregistered successfully");
//             }
//             catch (Exception ex)
//             {
//                 logger.LogError(ex, "Failed to deregister service from Consul");
//             }
//         });
//
//         return app;
//     }
//
//     public static IServiceCollection AddConsulClient(this IServiceCollection services, IConfiguration configuration)
//     {
//         services.AddSingleton<IConsulClient>(sp =>
//         {
//             var consulHost = configuration["Consul:Host"] ?? "localhost";
//             var consulPort = int.Parse(configuration["Consul:Port"] ?? "8500");
//
//             return new ConsulClient(config =>
//             {
//                 config.Address = new Uri($"http://{consulHost}:{consulPort}");
//             });
//         });
//
//         return services;
//     }
// }

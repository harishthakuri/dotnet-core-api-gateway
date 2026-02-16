namespace ApiGateway.Api.Middleware;

public class ClientIdMiddleware
{
    private readonly RequestDelegate _next;

    public ClientIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Use client IP address as the client identifier for rate limiting
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        // Add X-Real-IP header for Ocelot rate limiting
        if (!context.Request.Headers.ContainsKey("X-Real-IP"))
        {
            context.Request.Headers.Append("X-Real-IP", clientIp);
        }

        await _next(context);
    }
}

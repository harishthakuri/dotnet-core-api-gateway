namespace ApiGateway.Api.Middleware;

public class HealthCheckMiddleware
{
    private readonly RequestDelegate _next;

    public HealthCheckMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.Equals("/health", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            
            var response = new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                service = "API Gateway"
            };
            
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        await _next(context);
    }
}

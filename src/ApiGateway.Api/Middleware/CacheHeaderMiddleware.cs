namespace ApiGateway.Api.Middleware;

public class CacheHeaderMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CacheHeaderMiddleware> _logger;

    public CacheHeaderMiddleware(RequestDelegate next, ILogger<CacheHeaderMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Store original body stream
        var originalBodyStream = context.Response.Body;

        try
        {
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Add timestamp before request
            var requestTime = DateTime.UtcNow;

            // Call the next middleware
            await _next(context);

            // Add cache indicators to response headers
            if (context.Response.StatusCode == 200)
            {
                var responseTime = DateTime.UtcNow;
                var duration = (responseTime - requestTime).TotalMilliseconds;

                // If response was very fast (< 10ms), likely from cache
                var fromCache = duration < 10;

                context.Response.Headers.Append("X-Cache-Status", fromCache ? "HIT" : "MISS");
                context.Response.Headers.Append("X-Response-Time", $"{duration:F2}ms");
                
                _logger.LogInformation(
                    "Request {Method} {Path} - Cache: {CacheStatus}, Duration: {Duration}ms",
                    context.Request.Method,
                    context.Request.Path,
                    fromCache ? "HIT" : "MISS",
                    duration);
            }

            // Copy the response body back to the original stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}

using ApiGateway.Api.Services;
using Microsoft.AspNetCore.Authentication;

namespace ApiGateway.Api.Middleware;

public class TokenTransformationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenTransformationMiddleware> _logger;

    public TokenTransformationMiddleware(
        RequestDelegate next,
        ILogger<TokenTransformationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITokenTransformationService tokenService)
    {
        // Only transform token for authenticated requests
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            try
            {
                // Get the original Auth0 token from the Authorization header
                var authHeader = context.Request.Headers["Authorization"].ToString();
                var originalToken = authHeader.Replace("Bearer ", "").Trim();

                // Generate internal token from external claims
                var internalToken = tokenService.GenerateInternalToken(context.User);

                // Replace the Authorization header with internal token
                context.Request.Headers["Authorization"] = $"Bearer {internalToken}";

                // Optionally, add original token as a custom header for auditing
                if (!string.IsNullOrEmpty(originalToken))
                {
                    context.Request.Headers["X-Original-Authorization"] = $"Bearer {originalToken}";
                }

                _logger.LogDebug("Transformed external token to internal token for user: {UserId}", 
                    context.User.FindFirst("sub")?.Value ?? "Unknown");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transforming token");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Token transformation failed");
                return;
            }
        }

        await _next(context);
    }
}
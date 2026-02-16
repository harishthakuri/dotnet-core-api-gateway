using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ApiGateway.Api.Services;

public class TokenTransformationService:ITokenTransformationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenTransformationService> _logger;

    public TokenTransformationService(
        IConfiguration configuration,
        ILogger<TokenTransformationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    
    public string GenerateInternalToken(ClaimsPrincipal externalUser)
    {
        var issuer = _configuration["Authentication:InternalToken:Issuer"];
        var audience = _configuration["Authentication:InternalToken:Audience"];
        var secretKey = _configuration["Authentication:InternalToken:SecretKey"];
        var expirationMinutes = int.Parse(_configuration["Authentication:InternalToken:ExpirationMinutes"] ?? "60");

        // Normalize claims from different providers
        var normalizedClaims = NormalizeClaims(externalUser);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(normalizedClaims),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        _logger.LogInformation("Generated internal token for user: {UserId}", normalizedClaims.FirstOrDefault(c => c.Type == "user_id")?.Value);

        return tokenString;
    }

    private List<Claim> NormalizeClaims(ClaimsPrincipal externalUser)
    {
        var claims = new List<Claim>();

        // Determine the provider based on issuer
        var issuer = externalUser.FindFirst("iss")?.Value ?? string.Empty;
        string provider;

        if (issuer.Contains("auth0.com"))
        {
            provider = "auth0";
        }
        else if (issuer.Contains("accounts.google.com"))
        {
            provider = "google";
        }
        else
        {
            provider = "unknown";
        }

        // Normalize User ID
        var userId = externalUser.FindFirst("sub")?.Value
                     ?? externalUser.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? externalUser.FindFirst("oid")?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            claims.Add(new Claim("user_id", userId));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
        }

        // Normalize Email
        var email = externalUser.FindFirst("email")?.Value
                    ?? externalUser.FindFirst(ClaimTypes.Email)?.Value;

        if (!string.IsNullOrEmpty(email))
        {
            claims.Add(new Claim("email", email));
            claims.Add(new Claim(ClaimTypes.Email, email));
        }

        // Normalize Name
        var name = externalUser.FindFirst("name")?.Value
                   ?? externalUser.FindFirst(ClaimTypes.Name)?.Value
                   ?? externalUser.FindFirst("given_name")?.Value;

        if (!string.IsNullOrEmpty(name))
        {
            claims.Add(new Claim("name", name));
            claims.Add(new Claim(ClaimTypes.Name, name));
        }

        // Add provider information
        claims.Add(new Claim("auth_provider", provider));
        claims.Add(new Claim("original_issuer", issuer));

        // Add any custom scopes or permissions
        var scopes = externalUser.FindAll("scope");
        foreach (var scope in scopes)
        {
            claims.Add(new Claim("scope", scope.Value));
        }

        // Add roles if present
        var roles = externalUser.FindAll(ClaimTypes.Role);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Value));
        }

        _logger.LogDebug("Normalized claims for user from provider: {Provider}", provider);

        return claims;
    }
}
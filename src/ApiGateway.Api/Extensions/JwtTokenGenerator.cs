// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using Microsoft.IdentityModel.Tokens;
//
// namespace ApiGateway.Api.Extensions;
//
// /// <summary>
// /// Helper class to generate JWT tokens for testing
// /// </summary>
// public static class JwtTokenGenerator
// {
//     public static string GenerateToken(string secretKey, string issuer, string audience, int expirationMinutes = 60, string? userName = null, string[]? roles = null)
//     {
//         var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
//         var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
//
//         var claims = new List<Claim>
//         {
//             new(JwtRegisteredClaimNames.Sub, userName ?? "testuser"),
//             new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
//             new(ClaimTypes.Name, userName ?? "testuser")
//         };
//
//         if (roles != null)
//         {
//             foreach (var role in roles)
//             {
//                 claims.Add(new Claim(ClaimTypes.Role, role));
//             }
//         }
//
//         var token = new JwtSecurityToken(
//             issuer: issuer,
//             audience: audience,
//             claims: claims,
//             expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
//             signingCredentials: credentials
//         );
//
//         return new JwtSecurityTokenHandler().WriteToken(token);
//     }
// }

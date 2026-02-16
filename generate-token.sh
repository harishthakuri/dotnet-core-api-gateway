#!/bin/bash

# Generate a test JWT token for the API Gateway
# This script uses the same settings as appsettings.json

SECRET_KEY="your-secret-key-min-32-characters-long-for-production"
ISSUER="ApiGateway"
AUDIENCE="ApiGatewayClients"

# Build and run a small C# program to generate the token
cd src/ApiGateway.Api

# Create a temporary token generation program
cat > /tmp/generate-token.csx << 'EOF'
#r "nuget: System.IdentityModel.Tokens.Jwt, 8.0.1"

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var secretKey = Args[0];
var issuer = Args[1];
var audience = Args[2];

var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

var claims = new List<Claim>
{
    new(JwtRegisteredClaimNames.Sub, "testuser"),
    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    new(ClaimTypes.Name, "testuser"),
    new(ClaimTypes.Role, "Admin")
};

var token = new JwtSecurityToken(
    issuer: issuer,
    audience: audience,
    claims: claims,
    expires: DateTime.UtcNow.AddHours(1),
    signingCredentials: credentials
);

Console.WriteLine(new JwtSecurityTokenHandler().WriteToken(token));
EOF

echo "Generating JWT token..."
dotnet script /tmp/generate-token.csx "$SECRET_KEY" "$ISSUER" "$AUDIENCE"

cd ../..

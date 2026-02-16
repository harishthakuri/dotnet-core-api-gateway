
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


// Configure authentication to ONLY accept internal gateway tokens
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var issuer = builder.Configuration["Authentication:Gateway:Issuer"];
        var audience = builder.Configuration["Authentication:Gateway:Audience"];
        var secretKey = builder.Configuration["Authentication:Gateway:SecretKey"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint
app.MapGet("/health", () => new { status = "healthy", service = "ProductService" });

// Protected endpoint - requires authentication
// Protected endpoint with user info
app.MapGet("/user-info", [Authorize](HttpContext context) =>
    {
        var username = context.User.Identity?.Name;
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

        return Results.Ok(new
        {
            username,
            userId,
            role,
            claims = context.User.Claims.Select(c => new { c.Type, c.Value })
        });
    })
    .WithName("UserInfo");


// Products endpoint
app.MapGet("/products",[Authorize] () =>
{
    var products = new[]
    {
        new { Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics" },
        new { Id = 2, Name = "Mouse", Price = 29.99m, Category = "Electronics" },
        new { Id = 3, Name = "Keyboard", Price = 79.99m, Category = "Electronics" },
        new { Id = 4, Name = "Monitor", Price = 299.99m, Category = "Electronics" },
        new { Id = 5, Name = "Desk Chair", Price = 199.99m, Category = "Furniture" }
    };

    return Results.Ok(products);
}).WithName("GetProducts");

// Get product by ID
app.MapGet("/products/{id:int}", (int id) =>
{
    var products = new[]
    {
        new { Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics" },
        new { Id = 2, Name = "Mouse", Price = 29.99m, Category = "Electronics" },
        new { Id = 3, Name = "Keyboard", Price = 79.99m, Category = "Electronics" },
        new { Id = 4, Name = "Monitor", Price = 299.99m, Category = "Electronics" },
        new { Id = 5, Name = "Desk Chair", Price = 199.99m, Category = "Furniture" }
    };

    var product = products.FirstOrDefault(p => p.Id == id);
    
    return product is not null 
        ? Results.Ok(product) 
        : Results.NotFound(new { message = $"Product with ID {id} not found" });
});

app.Run();

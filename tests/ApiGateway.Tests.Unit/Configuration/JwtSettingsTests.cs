using ApiGateway.Api.Configuration;
using FluentAssertions;

namespace ApiGateway.Tests.Unit.Configuration;

public class JwtSettingsTests
{
    [Fact]
    public void JwtSettings_PropertiesCanBeSet()
    {
        // Arrange & Act
        var settings = new JwtSettings
        {
            SecretKey = "test-secret-key-with-minimum-length",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationMinutes = 30
        };

        // Assert
        settings.SecretKey.Should().Be("test-secret-key-with-minimum-length");
        settings.Issuer.Should().Be("TestIssuer");
        settings.Audience.Should().Be("TestAudience");
        settings.ExpirationMinutes.Should().Be(30);
    }

    [Fact]
    public void JwtSettings_DefaultValues()
    {
        // Arrange & Act
        var settings = new JwtSettings();

        // Assert
        settings.SecretKey.Should().BeEmpty();
        settings.Issuer.Should().BeEmpty();
        settings.Audience.Should().BeEmpty();
        settings.ExpirationMinutes.Should().Be(60); // Default value from class
    }
}

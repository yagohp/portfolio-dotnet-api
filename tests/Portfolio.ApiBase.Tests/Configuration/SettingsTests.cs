using FluentAssertions;
using Portfolio.ApiBase.Configuration; // Ajuste o namespace se necessário
using Xunit;

namespace Portfolio.ApiBase.Tests.Configuration;

public class SettingsTests
{
    [Fact]
    public void CorsSettings_ShouldBeMutableRecordClass()
    {
        // Arrange
        var settings = new CorsSettings(); // Se fosse record struct, isso poderia falhar dependendo do contexto do binder

        // Act
        settings.AllowedOrigins = new[] { "https://localhost:3000" };

        // Assert
        settings.AllowedOrigins.Should().ContainSingle().Which.Should().Be("https://localhost:3000");
    }
}
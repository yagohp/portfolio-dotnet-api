using FluentAssertions;
using Portfolio.Domain.Entities;
using Xunit;

namespace Portfolio.Domain.Tests.Entities;

public class RefreshTokenTests
{
    [Theory]
    [InlineData(-1, true)]  // Expirou 1 min atrás
    [InlineData(0, true)]   // Expira exatamente agora (Borda: <=)
    [InlineData(1, false)]  // Expira daqui a 1 min
    public void IsExpiredAt_ShouldEvaluateCorrectly(int minutesOffset, bool expectedIsExpired)
    {
        // Arrange
        var now = DateTime.UtcNow;
        var token = new RefreshToken { ExpiresAt = now.AddMinutes(minutesOffset) };

        // Act
        var result = token.IsExpiredAt(now);

        // Assert
        result.Should().Be(expectedIsExpired);
    }

    [Fact]
    public void IsActiveAt_ShouldReturnTrue_WhenTokenIsNotRevokedAndNotExpired()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var token = new RefreshToken { ExpiresAt = now.AddMinutes(10) };

        // Act
        var result = token.IsActiveAt(now);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsActiveAt_ShouldReturnFalse_WhenTokenIsRevoked()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var token = new RefreshToken { ExpiresAt = now.AddMinutes(10) };
        
        // Act
        token.Revoke(now, "127.0.0.1");

        // Assert
        token.IsActiveAt(now).Should().BeFalse();
        token.RevokedAt.Should().Be(now);
        token.RevokedByIp.Should().Be("127.0.0.1");
    }

    [Fact]
    public void Revoke_ShouldSetReplacementHash_WhenRotated()
    {
        // Arrange
        var token = new RefreshToken();
        var now = DateTime.UtcNow;

        // Act
        token.Revoke(now, "192.168.0.1", "new_hash_123");

        // Assert
        token.RevokedAt.Should().Be(now);
        token.RevokedByIp.Should().Be("192.168.0.1");
        token.ReplacedByTokenHash.Should().Be("new_hash_123");
    }
}
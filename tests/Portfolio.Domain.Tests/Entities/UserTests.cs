using FluentAssertions;
using Portfolio.Domain.Entities;
using Xunit;

namespace Portfolio.Domain.Tests.Entities;

public class UserTests
{
    private const int MaxAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    [Fact]
    public void RegisterFailedAccess_ShouldIncrementCount_WhenBelowThreshold()
    {
        // Arrange
        var user = new User();
        
        // Act
        user.RegisterFailedAccess(MaxAttempts, LockoutDuration);
        
        // Assert
        user.AccessFailedCount.Should().Be(1);
        user.IsLockedOut.Should().BeFalse();
        user.LockoutEnd.Should().BeNull();
    }

    [Fact]
    public void RegisterFailedAccess_ShouldLockAccount_WhenThresholdIsReached()
    {
        // Arrange
        var user = new User();
        
        // Act
        for (int i = 0; i < MaxAttempts; i++)
        {
            user.RegisterFailedAccess(MaxAttempts, LockoutDuration);
        }
        
        // Assert
        user.AccessFailedCount.Should().Be(MaxAttempts);
        user.IsLockedOut.Should().BeTrue();
        user.LockoutEnd.Should().NotBeNull();
        user.LockoutEnd.Should().BeCloseTo(DateTime.UtcNow.Add(LockoutDuration), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RegisterSuccessfulLogin_ShouldResetFailedCountAndLockout()
    {
        // Arrange
        var user = new User();
        for (int i = 0; i < MaxAttempts; i++)
        {
            user.RegisterFailedAccess(MaxAttempts, LockoutDuration);
        }
        var loginTime = DateTime.UtcNow;
        
        // Act
        user.RegisterSuccessfulLogin(loginTime);
        
        // Assert
        user.AccessFailedCount.Should().Be(0);
        user.LockoutEnd.Should().BeNull();
        user.LastLoginAt.Should().Be(loginTime);
        user.IsLockedOut.Should().BeFalse();
    }

    [Theory]
    [InlineData(-1, true)]  // Expirou no passado (Bloqueado)
    [InlineData(1, false)]  // Expira no futuro (Não bloqueado)
    public void IsLockedOut_ShouldEvaluateCorrectly_BasedOnLockoutEnd(int minutesOffset, bool expectedIsLockedOut)
    {
        // Arrange
        var user = new User();
        
        // Simulando o estado de lockout via falhas
        for (int i = 0; i < MaxAttempts; i++)
        {
            user.RegisterFailedAccess(MaxAttempts, TimeSpan.FromMinutes(minutesOffset));
        }

        // Act
        var result = user.IsLockedOut;

        // Assert
        result.Should().Be(expectedIsLockedOut);
    }
}
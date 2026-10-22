namespace Portfolio.Domain.Entities;

/// <summary>
/// Represents an application user, including credentials, roles, and security states.
/// </summary>
public sealed class User : EntityMetadata
{
    public string Email { get; private set; } = default!;
    public string Username { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public List<string> Roles { get; private set; } = new();
    public bool EmailConfirmed { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public int AccessFailedCount { get; private set; }
    public DateTime? LockoutEnd { get; private set; }
    public bool TwoFactorEnabled { get; private set; }
    public string? TwoFactorSecret { get; private set; }
    public List<string> TwoFactorRecoveryCodes { get; private set; } = new();
    public DateTime? TwoFactorEnabledAt { get; private set; }

    /// <summary>
    /// Indicates whether the user account is currently locked out.
    /// </summary>
    public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;

    /// <summary>
    /// Registers a failed login attempt and locks the account if the threshold is reached.
    /// </summary>
    public void RegisterFailedAccess(int maxAttempts, TimeSpan lockoutDuration)
    {
        AccessFailedCount++;
        if (AccessFailedCount >= maxAttempts)
        {
            LockoutEnd = DateTime.UtcNow.Add(lockoutDuration);
        }
    }

    /// <summary>
    /// Resets the failed access count and updates the last login timestamp upon successful authentication.
    /// </summary>
    public void RegisterSuccessfulLogin(DateTime now)
    {
        AccessFailedCount = 0;
        LockoutEnd = null;
        LastLoginAt = now;
    }
}
namespace Portfolio.Domain.Entities;

/// <summary>
/// Represents a refresh token used to maintain a user's authenticated session.
/// </summary>
public sealed class RefreshToken : EntityMetadata
{
    /// <summary>
    /// The identifier of the user who owns this token.
    /// </summary>
    public string UserId { get; private set; } = default!;

    /// <summary>
    /// The cryptographic hash of the raw refresh token. Never store the raw token.
    /// </summary>
    public string TokenHash { get; private set; } = default!;

    /// <summary>
    /// The UTC date and time when this token expires.
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// The UTC date and time when this token was revoked (if applicable).
    /// </summary>
    public DateTime? RevokedAt { get; private set; }

    /// <summary>
    /// The hash of the new token that replaced this one during rotation (if applicable).
    /// </summary>
    public string? ReplacedByTokenHash { get; private set; }

    /// <summary>
    /// The IP address from which the token was created.
    /// </summary>
    public string? CreatedByIp { get; private set; }

    /// <summary>
    /// The IP address from which the token was revoked.
    /// </summary>
    public string? RevokedByIp { get; private set; }

    /// <summary>
    /// Determines whether the token has expired based on the provided current time.
    /// </summary>
    /// <param name="now">The current UTC date and time.</param>
    /// <returns>True if expired; otherwise, false.</returns>
    public bool IsExpiredAt(DateTime now) => ExpiresAt <= now;

    /// <summary>
    /// Determines whether the token is active (not revoked and not expired) at the given time.
    /// </summary>
    /// <param name="now">The current UTC date and time.</param>
    /// <returns>True if active; otherwise, false.</returns>
    public bool IsActiveAt(DateTime now) => RevokedAt is null && !IsExpiredAt(now);

    /// <summary>
    /// Revokes the current token and records the IP address and reason.
    /// </summary>
    /// <param name="revokedAt">The UTC date and time of revocation.</param>
    /// <param name="revokedByIp">The IP address from which the token was revoked.</param>
    /// <param name="replacedByTokenHash">The hash of the token replacing this one, if any.</param>
    public void Revoke(DateTime revokedAt, string? revokedByIp, string? replacedByTokenHash = null)
    {
        RevokedAt = revokedAt;
        RevokedByIp = revokedByIp;
        ReplacedByTokenHash = replacedByTokenHash;
    }
}
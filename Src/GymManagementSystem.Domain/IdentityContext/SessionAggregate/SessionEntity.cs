using Bonyan.Layer.Domain.Aggregate;
using System;

namespace GymManagementSystem.Domain.IdentityContext.SessionAggregate;

/// <summary>
/// Represents a user's login session with streamlined and clear behavior.
/// </summary>
public class SessionEntity : BonAggregateRoot<Guid>
{
    /// <summary>
    /// IP address associated with the session.
    /// </summary>
    public string IpAddress { get; private set; }

    /// <summary>
    /// Browser or device details used for the session.
    /// </summary>
    public string Device { get; private set; }

    /// <summary>
    /// Timestamp when the session was initiated.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Current status of the session (e.g., Active, Expired, Inactive).
    /// </summary>
    public UserSessionStatus Status { get; private set; }

    /// <summary>
    /// Timestamp of the last user activity during the session.
    /// </summary>
    public DateTime LastActivityAt { get; private set; }

    /// <summary>
    /// Timestamp of the last status update for the session.
    /// </summary>
    public DateTime LastUpdatedAt { get; private set; }

    protected SessionEntity() { } // For EF Core

    /// <summary>
    /// Initializes a new session instance.
    /// </summary>
    public SessionEntity(string ipAddress, string device)
    {
        if (string.IsNullOrWhiteSpace(ipAddress)) throw new ArgumentNullException(nameof(ipAddress));
        if (string.IsNullOrWhiteSpace(device)) throw new ArgumentNullException(nameof(device));

        Id = Guid.NewGuid();
        IpAddress = ipAddress;
        Device = device;
        CreatedAt = DateTime.UtcNow;
        Status = UserSessionStatus.Active;
        LastActivityAt = CreatedAt;
        LastUpdatedAt = CreatedAt;
    }

    /// <summary>
    /// Marks the session as active, updating the last activity timestamp.
    /// </summary>
    public void RefreshActivity()
    {
        if (Status != UserSessionStatus.Active)
            throw new InvalidOperationException("Cannot refresh activity for a non-active session.");

        LastActivityAt = DateTime.UtcNow;
        LastUpdatedAt = LastActivityAt;
    }

    /// <summary>
    /// Expires the session if it exceeds the allowed inactivity duration.
    /// </summary>
    public void ExpireIfInactive(TimeSpan maxInactivityDuration)
    {
        if (Status == UserSessionStatus.Active && HasExceededInactivityDuration(maxInactivityDuration))
        {
            ExpireSession();
        }
    }

    /// <summary>
    /// Marks the session as expired.
    /// </summary>
    public void ExpireSession()
    {
        Status = UserSessionStatus.Expired;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Ends the session, marking it as inactive.
    /// </summary>
    public void EndSession()
    {
        if (Status == UserSessionStatus.Expired)
            throw new InvalidOperationException("Cannot end an already expired session.");

        Status = UserSessionStatus.Inactive;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Handles re-login by reactivating an expired or inactive session.
    /// </summary>
    public void Relogin()
    {
        if (Status == UserSessionStatus.Active)
            throw new InvalidOperationException("Session is already active.");

        Status = UserSessionStatus.Active;
        CreatedAt = DateTime.UtcNow;
        LastActivityAt = CreatedAt;
        LastUpdatedAt = CreatedAt;
    }

    /// <summary>
    /// Checks whether the session is currently active.
    /// </summary>
    public bool IsActive() => Status == UserSessionStatus.Active;

    /// <summary>
    /// Checks whether the session is expired.
    /// </summary>
    public bool IsExpired() => Status == UserSessionStatus.Expired;

    /// <summary>
    /// Checks whether the session is inactive.
    /// </summary>
    public bool IsInactive() => Status == UserSessionStatus.Inactive;

    /// <summary>
    /// Determines if the session has exceeded the maximum allowed inactivity duration.
    /// </summary>
    private bool HasExceededInactivityDuration(TimeSpan maxInactivityDuration)
    {
        return (DateTime.UtcNow - LastActivityAt) > maxInactivityDuration;
    }

    /// <summary>
    /// Gets the duration of the session since it was initiated.
    /// </summary>
    public TimeSpan GetSessionDuration()
    {
        return DateTime.UtcNow - CreatedAt;
    }

    /// <summary>
    /// Gets the duration since the last activity in the session.
    /// </summary>
    public TimeSpan GetInactivityDuration()
    {
        return DateTime.UtcNow - LastActivityAt;
    }

    /// <summary>
    /// Gets the duration since the last status update.
    /// </summary>
    public TimeSpan GetDurationSinceLastUpdate()
    {
        return DateTime.UtcNow - LastUpdatedAt;
    }
}

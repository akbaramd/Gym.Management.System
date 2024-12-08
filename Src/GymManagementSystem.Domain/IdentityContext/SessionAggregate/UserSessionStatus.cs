using Bonyan.Layer.Domain.Enumerations;

namespace GymManagementSystem.Domain.IdentityContext.SessionAggregate;

/// <summary>
/// Represents statuses for user sessions within the identity bounded context.
/// </summary>
public class UserSessionStatus : BonEnumeration
{
    /// <summary>
    /// Active session status: The user session is currently active and valid.
    /// </summary>
    public static readonly UserSessionStatus Active = new UserSessionStatus(ActiveId, ActiveName);
    public const string ActiveName = nameof(Active);
    public const int ActiveId = 0;

    /// <summary>
    /// Inactive session status: The user session is inactive or expired.
    /// </summary>
    public static readonly UserSessionStatus Inactive = new UserSessionStatus(InactiveId, InactiveName);
    public const string InactiveName = nameof(Inactive);
    public const int InactiveId = 1;

    /// <summary>
    /// Suspended session status: The user session is temporarily suspended due to violations or issues.
    /// </summary>
    public static readonly UserSessionStatus Suspended = new UserSessionStatus(SuspendedId, SuspendedName);
    public const string SuspendedName = nameof(Suspended);
    public const int SuspendedId = 2;

    /// <summary>
    /// Terminated session status: The user session has been explicitly terminated by the user or the system.
    /// </summary>
    public static readonly UserSessionStatus Terminated = new UserSessionStatus(TerminatedId, TerminatedName);
    public const string TerminatedName = nameof(Terminated);
    public const int TerminatedId = 3;

    /// <summary>
    /// Pending session status: The user session is waiting for further verification or confirmation.
    /// </summary>
    public static readonly UserSessionStatus Pending = new UserSessionStatus(PendingId, PendingName);
    public const string PendingName = nameof(Pending);
    public const int PendingId = 4;

    /// <summary>
    /// Locked session status: The session is locked due to repeated failed login attempts or other security reasons.
    /// </summary>
    public static readonly UserSessionStatus Locked = new UserSessionStatus(LockedId, LockedName);
    public const string LockedName = nameof(Locked);
    public const int LockedId = 5;

    /// <summary>
    /// Expired session status: The session has exceeded its validity period and is no longer active.
    /// </summary>
    public static readonly UserSessionStatus Expired = new UserSessionStatus(ExpiredId, ExpiredName);
    public const string ExpiredName = nameof(Expired);
    public const int ExpiredId = 6;

    /// <summary>
    /// Unauthorized session status: The session was created but lacks the necessary authorization.
    /// </summary>
    public static readonly UserSessionStatus Unauthorized = new UserSessionStatus(UnauthorizedId, UnauthorizedName);
    public const string UnauthorizedName = nameof(Unauthorized);
    public const int UnauthorizedId = 7;

    /// <summary>
    /// Constructor for creating a new session status.
    /// </summary>
    /// <param name="id">The unique ID for the status.</param>
    /// <param name="name">The name of the status.</param>
    private UserSessionStatus(int id, string name) : base(id, name) { }
}

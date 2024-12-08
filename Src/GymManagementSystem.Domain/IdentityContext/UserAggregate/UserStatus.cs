using Bonyan.Layer.Domain.Enumerations;

namespace GymManagementSystem.Domain.IdentityContext.UserAggregate;

/// <summary>
/// Represents user statuses in the identity bounded context for comprehensive control.
/// </summary>
public class UserStatus : BonEnumeration
{
    // Active user status: User is active and has full access.
    public static UserStatus Active = new UserStatus(ActiveId, ActiveName);
    public const string ActiveName = nameof(Active);
    public const int ActiveId = 0;

    // Inactive user status: User account exists but is not currently active.
    public static UserStatus Inactive = new UserStatus(InactiveId, InactiveName);
    public const string InactiveName = nameof(Inactive);
    public const int InactiveId = 1;

    // Suspended user status: User access temporarily blocked due to violations or issues.
    public static UserStatus Suspended = new UserStatus(SuspendedId, SuspendedName);
    public const string SuspendedName = nameof(Suspended);
    public const int SuspendedId = 2;

    // Banned user status: User permanently blocked from the system.
    public static UserStatus Banned = new UserStatus(BannedId, BannedName);
    public const string BannedName = nameof(Banned);
    public const int BannedId = 3;

    // Pending verification: User has registered but has not completed verification.
    public static UserStatus PendingVerification = new UserStatus(PendingVerificationId, PendingVerificationName);
    public const string PendingVerificationName = nameof(PendingVerification);
    public const int PendingVerificationId = 4;

    // Locked: User account locked due to repeated failed login attempts.
    public static UserStatus Locked = new UserStatus(LockedId, LockedName);
    public const string LockedName = nameof(Locked);
    public const int LockedId = 5;


    /// <summary>
    /// Constructor for creating a new user status.
    /// </summary>
    /// <param name="id">The unique ID for the status.</param>
    /// <param name="name">The name of the status.</param>
    public UserStatus(int id, string name) : base(id, name)
    {
    }
}

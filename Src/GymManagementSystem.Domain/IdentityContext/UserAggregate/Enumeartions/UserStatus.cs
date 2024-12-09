namespace GymManagementSystem.Domain.IdentityContext.UserAggregate;

/// <summary>
///   Represents user statuses in the identity bounded context for comprehensive control.
/// </summary>
public class UserStatus : BonEnumeration
{
  public const string ActiveName = nameof(Active);
  public const int ActiveId = 0;
  public const string InactiveName = nameof(Inactive);
  public const int InactiveId = 1;
  public const string SuspendedName = nameof(Suspended);
  public const int SuspendedId = 2;
  public const string BannedName = nameof(Banned);
  public const int BannedId = 3;
  public const string PendingVerificationName = nameof(PendingVerification);
  public const int PendingVerificationId = 4;
  public const string LockedName = nameof(Locked);

  public const int LockedId = 5;

  // Active user status: User is active and has full access.
  public static UserStatus Active = new(ActiveId, ActiveName);

  // Inactive user status: User account exists but is not currently active.
  public static UserStatus Inactive = new(InactiveId, InactiveName);

  // Suspended user status: User access temporarily blocked due to violations or issues.
  public static UserStatus Suspended = new(SuspendedId, SuspendedName);

  // Banned user status: User permanently blocked from the system.
  public static UserStatus Banned = new(BannedId, BannedName);

  // Pending verification: User has registered but has not completed verification.
  public static UserStatus PendingVerification = new(PendingVerificationId, PendingVerificationName);

  // Locked: User account locked due to repeated failed login attempts.
  public static UserStatus Locked = new(LockedId, LockedName);


  /// <summary>
  ///   Constructor for creating a new user status.
  /// </summary>
  /// <param name="id">The unique ID for the status.</param>
  /// <param name="name">The name of the status.</param>
  public UserStatus(int id, string name) : base(id, name)
  {
  }
}

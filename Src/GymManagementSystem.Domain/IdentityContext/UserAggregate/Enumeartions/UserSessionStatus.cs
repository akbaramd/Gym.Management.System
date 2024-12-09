namespace GymManagementSystem.Domain.IdentityContext.SessionAggregate;

/// <summary>
///   Represents statuses for user sessions within the identity bounded context.
/// </summary>
public class UserSessionStatus : BonEnumeration
{
  public const string ActiveName = nameof(Active);
  public const int ActiveId = 0;
  public const string InactiveName = nameof(Inactive);
  public const int InactiveId = 1;
  public const string SuspendedName = nameof(Suspended);
  public const int SuspendedId = 2;
  public const string TerminatedName = nameof(Terminated);
  public const int TerminatedId = 3;
  public const string PendingName = nameof(Pending);
  public const int PendingId = 4;
  public const string LockedName = nameof(Locked);
  public const int LockedId = 5;
  public const string ExpiredName = nameof(Expired);
  public const int ExpiredId = 6;
  public const string UnauthorizedName = nameof(Unauthorized);
  public const int UnauthorizedId = 7;

  /// <summary>
  ///   Active session status: The user session is currently active and valid.
  /// </summary>
  public static readonly UserSessionStatus Active = new(ActiveId, ActiveName);

  /// <summary>
  ///   Inactive session status: The user session is inactive or expired.
  /// </summary>
  public static readonly UserSessionStatus Inactive = new(InactiveId, InactiveName);

  /// <summary>
  ///   Suspended session status: The user session is temporarily suspended due to violations or issues.
  /// </summary>
  public static readonly UserSessionStatus Suspended = new(SuspendedId, SuspendedName);

  /// <summary>
  ///   Terminated session status: The user session has been explicitly terminated by the user or the system.
  /// </summary>
  public static readonly UserSessionStatus Terminated = new(TerminatedId, TerminatedName);

  /// <summary>
  ///   Pending session status: The user session is waiting for further verification or confirmation.
  /// </summary>
  public static readonly UserSessionStatus Pending = new(PendingId, PendingName);

  /// <summary>
  ///   Locked session status: The session is locked due to repeated failed login attempts or other security reasons.
  /// </summary>
  public static readonly UserSessionStatus Locked = new(LockedId, LockedName);

  /// <summary>
  ///   Expired session status: The session has exceeded its validity period and is no longer active.
  /// </summary>
  public static readonly UserSessionStatus Expired = new(ExpiredId, ExpiredName);

  /// <summary>
  ///   Unauthorized session status: The session was created but lacks the necessary authorization.
  /// </summary>
  public static readonly UserSessionStatus Unauthorized = new(UnauthorizedId, UnauthorizedName);

  /// <summary>
  ///   Constructor for creating a new session status.
  /// </summary>
  /// <param name="id">The unique ID for the status.</param>
  /// <param name="name">The name of the status.</param>
  private UserSessionStatus(int id, string name) : base(id, name) { }
}

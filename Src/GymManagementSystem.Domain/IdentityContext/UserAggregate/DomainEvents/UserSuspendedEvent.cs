namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

/// <summary>
///   Event triggered when a user is suspended.
/// </summary>
public class UserSuspendedEvent : BonDomainEventBase
{
  public UserSuspendedEvent(Guid userId)
  {
    UserId = userId;
  }

  public Guid UserId { get; }
}

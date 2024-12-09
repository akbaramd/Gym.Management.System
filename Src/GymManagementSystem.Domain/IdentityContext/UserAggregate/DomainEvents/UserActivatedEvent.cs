namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

/// <summary>
///   Event triggered when a user is activated.
/// </summary>
public class UserActivatedEvent : BonDomainEventBase
{
  public UserActivatedEvent(Guid userId)
  {
    UserId = userId;
  }

  public Guid UserId { get; }
}

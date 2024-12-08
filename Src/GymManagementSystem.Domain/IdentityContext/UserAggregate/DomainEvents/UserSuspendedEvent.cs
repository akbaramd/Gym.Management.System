using Bonyan.Layer.Domain.Events;

namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

/// <summary>
/// Event triggered when a user is suspended.
/// </summary>
public class UserSuspendedEvent : BonDomainEventBase
{
  public Guid UserId { get; }

  public UserSuspendedEvent(Guid userId)
  {
    UserId = userId;
  }
}
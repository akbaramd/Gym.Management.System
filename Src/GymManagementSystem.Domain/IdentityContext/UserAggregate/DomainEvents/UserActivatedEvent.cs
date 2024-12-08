using Bonyan.Layer.Domain.Events;

namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

/// <summary>
/// Event triggered when a user is activated.
/// </summary>
public class UserActivatedEvent : BonDomainEventBase
{
  public Guid UserId { get; }

  public UserActivatedEvent(Guid userId)
  {
    UserId = userId;
  }
}
using Bonyan.Layer.Domain.Events;

namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

public class UserDeletedEvent : BonDomainEventBase
{
  public Guid UserId { get; }

  public UserDeletedEvent(Guid userId)
  {
    UserId = userId;
  }
}
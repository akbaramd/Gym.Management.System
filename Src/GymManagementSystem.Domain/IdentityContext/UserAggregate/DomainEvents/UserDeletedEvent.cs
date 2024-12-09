namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

public class UserDeletedEvent : BonDomainEventBase
{
  public UserDeletedEvent(Guid userId)
  {
    UserId = userId;
  }

  public Guid UserId { get; }
}

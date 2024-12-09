namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

public class UserProfileUpdatedEvent : BonDomainEventBase
{
  public UserProfileUpdatedEvent(Guid userId, string firstName, string lastName, MediaVo? profile)
  {
    UserId = userId;
    FirstName = firstName;
    LastName = lastName;
    Profile = profile;
  }

  public Guid UserId { get; }
  public string FirstName { get; }
  public string LastName { get; }
  public MediaVo? Profile { get; }
}

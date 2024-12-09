namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

/// <summary>
///   Event triggered when a user is created.
/// </summary>
public class UserCreatedEvent : BonDomainEventBase
{
  public UserCreatedEvent(Guid userId, string phoneNumber)
  {
    UserId = userId;
    PhoneNumber = phoneNumber;
  }

  public Guid UserId { get; }
  public string PhoneNumber { get; }
}

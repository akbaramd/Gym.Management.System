namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

public class UserPhoneNumberUpdatedEvent : BonDomainEventBase
{
  public UserPhoneNumberUpdatedEvent(Guid userId, string newAvatar)
  {
    UserId = userId;
    PhoneNumber = newAvatar;
  }

  public Guid UserId { get; }
  public string PhoneNumber { get; }
}

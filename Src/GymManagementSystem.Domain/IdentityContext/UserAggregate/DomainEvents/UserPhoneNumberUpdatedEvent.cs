using Bonyan.Layer.Domain.Events;
using GymManagementSystem.Shared.Domain.ValueObjects;

namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

public class UserPhoneNumberUpdatedEvent : BonDomainEventBase
{
  public Guid UserId { get; }
  public string PhoneNumber { get; }

  public UserPhoneNumberUpdatedEvent(Guid userId, string newAvatar)
  {
    UserId = userId;
    PhoneNumber = newAvatar;
  }
}
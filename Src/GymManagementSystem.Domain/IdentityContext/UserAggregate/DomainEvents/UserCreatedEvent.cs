using Bonyan.Layer.Domain.Events;

namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

/// <summary>
/// Event triggered when a user is created.
/// </summary>
public class UserCreatedEvent : BonDomainEventBase
{
    public Guid UserId { get; }
    public string PhoneNumber { get; }

    public UserCreatedEvent(Guid userId, string phoneNumber)
    {
        UserId = userId;
        PhoneNumber = phoneNumber;
    }
}
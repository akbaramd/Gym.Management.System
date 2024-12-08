using Bonyan.Layer.Domain.Events;

namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

public class UserRoleUnassignedEvent : BonDomainEventBase
{
  public Guid UserId { get; }
  public Guid RoleId { get; }

  public UserRoleUnassignedEvent(Guid userId, Guid roleId)
  {
    UserId = userId;
    RoleId = roleId;
  }
}
using Bonyan.Layer.Domain.Events;

namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

/// <summary>
/// Event triggered when a role is assigned to a user.
/// </summary>
public class UserRoleAssignedEvent : BonDomainEventBase
{
  public Guid UserId { get; }
  public Guid RoleId { get; }

  public UserRoleAssignedEvent(Guid userId, Guid roleId)
  {
    UserId = userId;
    RoleId = roleId;
  }
}
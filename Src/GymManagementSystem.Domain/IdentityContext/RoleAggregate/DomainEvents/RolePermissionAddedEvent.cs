using Bonyan.Layer.Domain.Events;

namespace GymManagementSystem.Domain.IdentityContext.RoleAggregate.DomainEvents;

/// <summary>
/// Event triggered when a permission is added to a role.
/// </summary>
public class RolePermissionAddedEvent : BonDomainEventBase
{
  public Guid RoleId { get; }
  public Guid PermissionId { get; }

  public RolePermissionAddedEvent(Guid roleId, Guid permissionId)
  {
    RoleId = roleId;
    PermissionId = permissionId;
  }
}
namespace GymManagementSystem.Domain.IdentityContext.RoleAggregate.DomainEvents;

/// <summary>
///   Event triggered when a permission is added to a role.
/// </summary>
public class RolePermissionAddedEvent : BonDomainEventBase
{
  public RolePermissionAddedEvent(Guid roleId, Guid permissionId)
  {
    RoleId = roleId;
    PermissionId = permissionId;
  }

  public Guid RoleId { get; }
  public Guid PermissionId { get; }
}

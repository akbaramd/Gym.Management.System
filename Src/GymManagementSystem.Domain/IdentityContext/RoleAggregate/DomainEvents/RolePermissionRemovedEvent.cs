namespace GymManagementSystem.Domain.IdentityContext.RoleAggregate.DomainEvents;

/// <summary>
///   Event triggered when a permission is removed from a role.
/// </summary>
public class RolePermissionRemovedEvent : BonDomainEventBase
{
  public RolePermissionRemovedEvent(Guid roleId, Guid permissionId)
  {
    RoleId = roleId;
    PermissionId = permissionId;
  }

  public Guid RoleId { get; }
  public Guid PermissionId { get; }
}

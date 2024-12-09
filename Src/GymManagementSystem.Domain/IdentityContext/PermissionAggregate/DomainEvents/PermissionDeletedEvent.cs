namespace GymManagementSystem.Domain.IdentityContext.PermissionAggregate.DomainEvents;

/// <summary>
///   Event triggered when a permission is marked as deleted.
/// </summary>
public class PermissionDeletedEvent : BonDomainEventBase
{
  public PermissionDeletedEvent(Guid permissionId, string title, string name)
  {
    PermissionId = permissionId;
    Title = title;
    Name = name;
  }

  public Guid PermissionId { get; }
  public string Title { get; }
  public string Name { get; }
}

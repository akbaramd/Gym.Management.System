namespace GymManagementSystem.Domain.IdentityContext.PermissionAggregate.DomainEvents;

/// <summary>
///   Event triggered when a permission is updated.
/// </summary>
public class PermissionUpdatedEvent : BonDomainEventBase
{
  public PermissionUpdatedEvent(Guid permissionId, string title, string name)
  {
    PermissionId = permissionId;
    Title = title;
    Name = name;
  }

  public Guid PermissionId { get; }
  public string Title { get; }
  public string Name { get; }
}

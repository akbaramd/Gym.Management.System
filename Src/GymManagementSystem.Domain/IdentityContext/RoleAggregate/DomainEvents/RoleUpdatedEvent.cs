using Bonyan.Layer.Domain.Events;

namespace GymManagementSystem.Domain.IdentityContext.RoleAggregate.DomainEvents;

/// <summary>
/// Event triggered when a permission is removed from a role.
/// </summary>
public class RoleUpdatedEvent : BonDomainEventBase
{
  public Guid RoleId { get; }
  public string Name { get; }
  public string Title { get; }

  public RoleUpdatedEvent(Guid roleId, string name, string title)
  {
    RoleId = roleId;
    Name = name;
    Title = title;
  }
}
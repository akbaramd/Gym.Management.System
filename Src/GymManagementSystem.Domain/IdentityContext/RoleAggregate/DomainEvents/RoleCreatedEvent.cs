using Bonyan.Layer.Domain.Events;

namespace GymManagementSystem.Domain.IdentityContext.RoleAggregate.DomainEvents;

/// <summary>
/// Event triggered when a role is created.
/// </summary>
public class RoleCreatedEvent : BonDomainEventBase
{
  public Guid RoleId { get; }
  public string Name { get; }
  public string Title { get; }

  public RoleCreatedEvent(Guid roleId, string name, string title)
  {
    RoleId = roleId;
    Name = name;
    Title = title;
  }
}
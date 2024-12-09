namespace GymManagementSystem.Domain.IdentityContext.RoleAggregate.DomainEvents;

/// <summary>
///   Event triggered when a role is created.
/// </summary>
public class RoleCreatedEvent : BonDomainEventBase
{
  public RoleCreatedEvent(Guid roleId, string name, string title)
  {
    RoleId = roleId;
    Name = name;
    Title = title;
  }

  public Guid RoleId { get; }
  public string Name { get; }
  public string Title { get; }
}

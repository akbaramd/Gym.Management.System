namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

public class UserRoleUnassignedEvent : BonDomainEventBase
{
  public UserRoleUnassignedEvent(Guid userId, Guid roleId)
  {
    UserId = userId;
    RoleId = roleId;
  }

  public Guid UserId { get; }
  public Guid RoleId { get; }
}

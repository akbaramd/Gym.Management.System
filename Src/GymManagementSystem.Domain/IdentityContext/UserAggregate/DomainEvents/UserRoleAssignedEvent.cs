namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

/// <summary>
///   Event triggered when a role is assigned to a user.
/// </summary>
public class UserRoleAssignedEvent : BonDomainEventBase
{
  public UserRoleAssignedEvent(Guid userId, Guid roleId)
  {
    UserId = userId;
    RoleId = roleId;
  }

  public Guid UserId { get; }
  public Guid RoleId { get; }
}

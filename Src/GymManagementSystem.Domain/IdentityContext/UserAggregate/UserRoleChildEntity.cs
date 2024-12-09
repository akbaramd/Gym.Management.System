#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace GymManagementSystem.Domain.IdentityContext.UserAggregate;
/// <summary>
///   Child entity for user-role relationship.
/// </summary>
public class UserRoleChildEntity : BonEntity<Guid>
{
  private UserRoleChildEntity() { } // EF Core constructor

  public UserRoleChildEntity(Guid userId, Guid roleId)
  {
    UserId = userId;
    RoleId = roleId;
  }

  public Guid UserId { get; private set; }
  public Guid RoleId { get; private set; }
  public RoleEntity Role { get; set; }
}

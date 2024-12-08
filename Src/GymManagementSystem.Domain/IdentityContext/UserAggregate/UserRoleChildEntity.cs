using Bonyan.Layer.Domain.Entities;

namespace GymManagementSystem.Domain.IdentityContext.UserAggregate;

/// <summary>
/// Child entity for user-role relationship.
/// </summary>
public class UserRoleChildEntity : BonEntity<Guid>
{
  public  Guid UserId { get; private set; }
  public  Guid RoleId { get; private set; }

  private UserRoleChildEntity() { } // EF Core constructor

  public UserRoleChildEntity(Guid userId, Guid roleId)
  {
    Id = Guid.NewGuid();
    UserId = userId;
    RoleId = roleId;
  }
}

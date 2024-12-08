using Bonyan.Layer.Domain.Entities;
using GymManagementSystem.Domain.IdentityContext.RoleAggregate;

namespace GymManagementSystem.Domain.IdentityContext.UserAggregate;

/// <summary>
/// Child entity for user-role relationship.
/// </summary>
public class UserRoleChildEntity : BonEntity<Guid>
{
  public  Guid UserId { get; private set; }
  public  Guid RoleId { get; private set; }
  public RoleEntity Role { get; set; }

  private UserRoleChildEntity() { } // EF Core constructor

  public UserRoleChildEntity(Guid userId, Guid roleId)
  {
    UserId = userId;
    RoleId = roleId;
  }
}

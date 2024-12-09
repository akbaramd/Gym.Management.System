using GymManagementSystem.Domain.IdentityContext.PermissionAggregate;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace GymManagementSystem.Domain.IdentityContext.RoleAggregate;

/// <summary>
///   Role-permission relationship child entity.
/// </summary>
public class RolePermissionChildEntity : BonEntity<Guid>
{
  private RolePermissionChildEntity() { } // EF Core constructor

  public RolePermissionChildEntity(Guid roleId, Guid permissionId)
  {
    RoleId = roleId;
    PermissionId = permissionId;
  }

  public Guid PermissionId { get; private set; }
  public PermissionEntity Permission { get; }
  public Guid RoleId { get; private set; }
}

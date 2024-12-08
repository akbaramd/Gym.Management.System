using Bonyan.Layer.Domain.Entities;

namespace GymManagementSystem.Domain.IdentityContext.RoleAggregate;

/// <summary>
/// Role-permission relationship child entity.
/// </summary>
public class RolePermissionChildEntity : BonEntity<Guid>
{
  public  Guid PermissionId { get; private set; }
  public  Guid RoleId { get; private set; }

  private RolePermissionChildEntity() { } // EF Core constructor

  public RolePermissionChildEntity(Guid roleId, Guid permissionId)
  {
    Id = Guid.NewGuid();
    RoleId = roleId;
    PermissionId = permissionId;
  }
}
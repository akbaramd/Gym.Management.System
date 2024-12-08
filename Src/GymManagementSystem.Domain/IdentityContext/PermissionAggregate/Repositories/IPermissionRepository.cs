using Bonyan.Layer.Domain.Repository.Abstractions;

namespace GymManagementSystem.Domain.IdentityContext.PermissionAggregate.Repositories;


/// <summary>
/// Repository interface for permission-related data access.
/// </summary>
public interface IPermissionRepository : IBonRepository<PermissionEntity, Guid>
{
  /// <summary>
  /// Finds a permission by its name.
  /// </summary>
  /// <param name="name">The name of the permission.</param>
  /// <returns>The permission entity if found, otherwise null.</returns>
  Task<PermissionEntity?> FindByNameAsync(string name);

  /// <summary>
  /// Checks if a permission exists by its ID.
  /// </summary>
  /// <param name="permissionId">The ID of the permission.</param>
  /// <returns>True if the permission exists, otherwise false.</returns>
  Task<bool> ExistsAsync(Guid permissionId);
}
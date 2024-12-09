namespace GymManagementSystem.Domain.IdentityContext.RoleAggregate.Repositories;

/// <summary>
///   Repository interface for role-related data access.
/// </summary>
public interface IRoleRepository : IBonRepository<RoleEntity, Guid>
{
  /// <summary>
  ///   Finds a role by its name.
  /// </summary>
  /// <param name="name">The name of the role.</param>
  /// <returns>The role entity if found, otherwise null.</returns>
  Task<RoleEntity?> FindByNameAsync(string name);

  /// <summary>
  ///   Checks if a role exists by its ID.
  /// </summary>
  /// <param name="roleId">The ID of the role.</param>
  /// <returns>True if the role exists, otherwise false.</returns>
  Task<bool> ExistsAsync(Guid roleId);
}

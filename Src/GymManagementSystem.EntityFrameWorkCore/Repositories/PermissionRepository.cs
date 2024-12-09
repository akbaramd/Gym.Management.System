namespace GymManagementSystem.Infrastructure.Data.Repositories;

public class PermissionRepository : EfCoreBonRepository<PermissionEntity, Guid, GymManagementSystemDbContext>,
  IPermissionRepository
{
  public Task<PermissionEntity?> FindByNameAsync(string name)
  {
    return FindOneAsync(x => x.Name.Equals(name));
  }

  public Task<bool> ExistsAsync(Guid roleId)
  {
    return ExistsAsync(x => x.Id.Equals(roleId));
  }
}

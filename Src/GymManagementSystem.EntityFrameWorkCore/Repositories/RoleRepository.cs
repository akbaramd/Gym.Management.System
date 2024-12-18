﻿namespace GymManagementSystem.Infrastructure.Data.Repositories;

public class RoleRepository : EfCoreBonRepository<RoleEntity, Guid, GymManagementSystemDbContext>, IRoleRepository
{
  public Task<RoleEntity?> FindByNameAsync(string name)
  {
    return FindOneAsync(x => x.Name.Equals(name));
  }

  public Task<bool> ExistsAsync(Guid roleId)
  {
    return ExistsAsync(x => x.Id.Equals(roleId));
  }

  protected override IQueryable<RoleEntity> PrepareQuery(DbSet<RoleEntity> dbSet)
  {
    return base.PrepareQuery(dbSet).Include(x => x.Permissions).ThenInclude(x => x.Permission);
  }
}

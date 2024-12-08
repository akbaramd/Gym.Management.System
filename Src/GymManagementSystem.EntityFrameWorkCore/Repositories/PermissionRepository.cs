using Bonyan.Layer.Domain;
using GymManagementSystem.Domain.IdentityContext.PermissionAggregate;
using GymManagementSystem.Domain.IdentityContext.PermissionAggregate.Repositories;
using GymManagementSystem.Domain.IdentityContext.RoleAggregate;
using GymManagementSystem.Domain.IdentityContext.RoleAggregate.Repositories;
using GymManagementSystem.Domain.IdentityContext.SessionAggregate;
using GymManagementSystem.Domain.IdentityContext.SessionAggregate.Repositories;
using GymManagementSystem.Domain.IdentityContext.UserAggregate;
using GymManagementSystem.Domain.IdentityContext.UserAggregate.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Infrastructure.Data.Repositories;

public class PermissionRepository : EfCoreBonRepository<PermissionEntity,Guid,GymManagementSystemDbContext>,IPermissionRepository
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
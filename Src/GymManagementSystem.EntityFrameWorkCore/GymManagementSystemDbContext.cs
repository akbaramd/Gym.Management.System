using System.Reflection;
using Bonyan.EntityFrameworkCore;
using Bonyan.EntityFrameworkCore.Abstractions;
using GymManagementSystem.Domain.IdentityContext.PermissionAggregate;
using GymManagementSystem.Domain.IdentityContext.RoleAggregate;
using GymManagementSystem.Domain.IdentityContext.SessionAggregate;
using GymManagementSystem.Domain.IdentityContext.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Infrastructure.Data;

public class GymManagementSystemDbContext : BonDbContext<GymManagementSystemDbContext>,
     IBonDbContext<GymManagementSystemDbContext>
{
    public GymManagementSystemDbContext(DbContextOptions<GymManagementSystemDbContext> options) :
        base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserRoleChildEntity> UserRoles { get; set; }
    public DbSet<UserSessionChildEntity> UserSessions { get; set; }
    public DbSet<UserTokensChildEntity> UserTokens { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<RolePermissionChildEntity> RolePermissions { get; set; }
    public DbSet<PermissionEntity> Permissions { get; set; }
    public DbSet<SessionEntity> Sessions { get; set; }
    

}

namespace GymManagementSystem.Infrastructure.Data;

public class GymManagementSystemDbContext : BonDbContext<GymManagementSystemDbContext>,
  IBonDbContext<GymManagementSystemDbContext>
{
  public GymManagementSystemDbContext(DbContextOptions<GymManagementSystemDbContext> options) :
    base(options)
  {
  }

  public DbSet<UserEntity> Users { get; set; }
  public DbSet<UserRoleChildEntity> UserRoles { get; set; }
  public DbSet<UserTokensChildEntity> UserTokens { get; set; }
  public DbSet<RoleEntity> Roles { get; set; }
  public DbSet<RolePermissionChildEntity> RolePermissions { get; set; }
  public DbSet<PermissionEntity> Permissions { get; set; }
  public DbSet<UserSessionChildEntity> Sessions { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    base.OnModelCreating(modelBuilder);
  }
}

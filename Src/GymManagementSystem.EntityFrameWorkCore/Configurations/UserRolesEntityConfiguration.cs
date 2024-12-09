namespace GymManagementSystem.Infrastructure.Data.Configurations;

public class UserRolesEntityConfiguration : IEntityTypeConfiguration<UserRoleChildEntity>
{
  public void Configure(EntityTypeBuilder<UserRoleChildEntity> entity)
  {
    entity.HasKey(ur => ur.Id);
    entity.ConfigureByConvention();
    entity.HasOne<UserEntity>()
      .WithMany(u => u.UserRoles)
      .HasForeignKey(ur => ur.UserId)
      .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(ur => ur.Role)
      .WithMany()
      .HasForeignKey(ur => ur.RoleId)
      .OnDelete(DeleteBehavior.Cascade);

    entity.ToTable("UserRoles"); // Explicitly map to the shared table
  }
}

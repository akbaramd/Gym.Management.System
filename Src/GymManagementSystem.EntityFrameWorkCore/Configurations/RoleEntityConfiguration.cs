namespace GymManagementSystem.Infrastructure.Data.Configurations;

public class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
{
  public void Configure(EntityTypeBuilder<RoleEntity> builder)
  {
    builder.ToTable("Roles");
    builder.ConfigureByConvention();
    // Configure RoleEntity properties
    builder.HasKey(r => r.Id);
    builder.Property(r => r.Name).IsRequired().HasMaxLength(50);

    // Configure relationships with other entities
    builder.HasMany(r => r.Permissions)
      .WithOne()
      .HasForeignKey(rp => rp.RoleId)
      .OnDelete(DeleteBehavior.Cascade);



  }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymManagementSystem.Domain.IdentityContext.PermissionAggregate;
using GymManagementSystem.Domain.IdentityContext.RoleAggregate;

namespace GymManagementSystem.Infrastructure.Data.Configurations
{
    public class PermissionEntityConfiguration : IEntityTypeConfiguration<PermissionEntity>
    {
        public void Configure(EntityTypeBuilder<PermissionEntity> builder)
        {
            builder.ToTable("Permissions");

            builder.ConfigureByConvention();
            
            // Configure PermissionEntity properties
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);

            // Configure relationships with other entities
            builder.HasMany<RolePermissionChildEntity>()
                .WithOne(rp => rp.Permission)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
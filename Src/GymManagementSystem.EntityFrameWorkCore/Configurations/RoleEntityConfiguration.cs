using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymManagementSystem.Domain.IdentityContext.RoleAggregate;
using GymManagementSystem.Domain.IdentityContext.UserAggregate;

namespace GymManagementSystem.Infrastructure.Data.Configurations
{
    public class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
    {
        public void Configure(EntityTypeBuilder<RoleEntity> builder)
        {
            builder.ToTable("Roles");

            // Configure RoleEntity properties
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name).IsRequired().HasMaxLength(50);

            // Configure relationships with other entities
            builder.HasMany(r => r.Permissions)
                .WithOne()
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            
            
            // Configure relationships with other entities
            builder.HasMany<UserRoleChildEntity>()
                .WithOne(x=>x.Role)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
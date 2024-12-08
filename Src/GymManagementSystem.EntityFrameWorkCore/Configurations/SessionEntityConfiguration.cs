using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymManagementSystem.Domain.IdentityContext.SessionAggregate;
using GymManagementSystem.Domain.IdentityContext.UserAggregate;

namespace GymManagementSystem.Infrastructure.Data.Configurations
{
    public class SessionEntityConfiguration : IEntityTypeConfiguration<SessionEntity>
    {
        public void Configure(EntityTypeBuilder<SessionEntity> builder)
        {
            builder.ToTable("Sessions");

            // Configure SessionEntity properties
            builder.HasKey(s => s.Id);
            builder.Property(s => s.IpAddress).IsRequired().HasMaxLength(50);
            builder.Property(s => s.Device).IsRequired().HasMaxLength(100);
            builder.Property(s => s.CreatedAt).IsRequired();

            // Configure relationships with other entities
            builder.HasMany<UserSessionChildEntity>()
                .WithOne(us => us.Session)
                .HasForeignKey(us => us.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
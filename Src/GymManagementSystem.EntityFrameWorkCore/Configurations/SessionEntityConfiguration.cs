namespace GymManagementSystem.Infrastructure.Data.Configurations;

public class SessionEntityConfiguration : IEntityTypeConfiguration<UserSessionChildEntity>
{
  public void Configure(EntityTypeBuilder<UserSessionChildEntity> builder)
  {
    builder.ToTable("UserSessions");
    builder.ConfigureByConvention();
    // Configure UserSessionChildEntity properties
    builder.HasKey(s => s.Id);
    builder.Property(s => s.CreatedAt).IsRequired();


    // Configure the Avatar value object
    builder.OwnsOne(x => x.IpAddress, avatar =>
    {
      // Map each Avatar property to a column name with the correct max length and constraints
      avatar.Property(a => a.Value)
        .IsRequired()
        .HasMaxLength(500)
        .HasColumnName("IpAddress"); // Column name for the file path

     
    });
    // Configure the Avatar value object
    builder.OwnsOne(x => x.Device, avatar =>
    {
      // Map each Avatar property to a column name with the correct max length and constraints
      avatar.Property(a => a.Value)
        .IsRequired()
        .HasMaxLength(500)
        .HasColumnName("Device"); // Column name for the file path

     
    });
    // Configure relationships with other entities
    builder.HasOne<UserEntity>()
      .WithMany(us => us.UserSessions)
      .HasForeignKey(us => us.UserId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}

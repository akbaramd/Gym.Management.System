namespace GymManagementSystem.Infrastructure.Data.Configurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
  public void Configure(EntityTypeBuilder<UserEntity> builder)
  {
    builder.ToTable("Users");
    builder.ConfigureByConvention(); // Use conventions for basic setups

    // Configure the Avatar value object
    builder.OwnsOne(x => x.Avatar, avatar =>
    {
      // Map each Avatar property to a column name with the correct max length and constraints
      avatar.Property(a => a.FilePath)
        .IsRequired()
        .HasMaxLength(500)
        .HasColumnName("AvatarFilePath"); // Column name for the file path

      avatar.Property(a => a.WebPath)
        .HasMaxLength(100)
        .HasColumnName("AvatarWebPath"); // Column name for the web path

      avatar.Property(a => a.Extension)
        .HasMaxLength(100)
        .HasColumnName("AvatarExtension"); // Column name for the file extension

      avatar.Property(a => a.Size)
        .HasColumnName("AvatarSize") // Column name for the avatar size
        .HasMaxLength(100); // Max length for the avatar size (size can be stored as a string or number based on your preference)
    });

    // Configure UserEntity properties
    builder.HasKey(u => u.Id);
    builder.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(15);
    builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);


    builder.HasMany(u => u.UserSessions)
      .WithOne()
      .HasForeignKey(us => us.UserId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(u => u.UserTokens)
      .WithOne()
      .HasForeignKey(ut => ut.UserId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}

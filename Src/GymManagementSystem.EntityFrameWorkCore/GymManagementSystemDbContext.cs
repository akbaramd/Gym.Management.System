using Bonyan.EntityFrameworkCore;
using Bonyan.EntityFrameworkCore.Abstractions;
using GymManagementSystem.Domain.Authors;
using GymManagementSystem.Domain.Books;
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
        base.OnModelCreating(modelBuilder);

        // ConfigureByConvention: From Bonyan.EntityFrameworkCore to apply default conventions of the domain.
        modelBuilder.Entity<BookEntity>().ConfigureByConvention();
        modelBuilder.Entity<AuthorEntity>().ConfigureByConvention();


        // Configures the relationship between BookEntity and AuthorEntity.
        modelBuilder.Entity<BookEntity>()
            .HasOne(book => book.AuthorEntity) // A Book is related to an Author.
            .WithMany() // An Author can have multiple Books.
            .HasForeignKey(book => book.AuthorId); // ForeignKey is AuthorId in BookEntity.
        
        // Configure BookEntity's owned value object
        modelBuilder.Entity<BookEntity>().OwnsOne(
            book => book.Detail,
            detail =>
            {
                detail.Property(d => d.Author).HasColumnName("Author");
                detail.Property(d => d.PublishedDate).HasColumnName("PublishedDate");
                detail.Property(d => d.ISBN).HasColumnName("ISBN");
                detail.Property(d => d.Pages).HasColumnName("Pages");
            });

    }

    // DbSet properties for domain entities.
    public DbSet<BookEntity> Books { get; set; }
    public DbSet<AuthorEntity> Authors { get; set; }
}

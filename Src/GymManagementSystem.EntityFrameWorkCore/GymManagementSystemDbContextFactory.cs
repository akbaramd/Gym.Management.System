using Microsoft.EntityFrameworkCore.Design;

namespace GymManagementSystem.Infrastructure.Data;

/// <summary>
///   Factory for creating instances of <see cref="GymManagementSystemDbContext" /> at design time.
///   This is used by tools like Entity Framework Core for migrations and scaffolding.
/// </summary>
public class GymManagementSystemDbContextFactory : IDesignTimeDbContextFactory<GymManagementSystemDbContext>
{
  /// <summary>
  ///   Creates a new instance of <see cref="GymManagementSystemDbContext" /> with the specified options.
  ///   This method is invoked during design time to configure the database context.
  /// </summary>
  /// <param name="args">Command-line arguments passed during the invocation.</param>
  /// <returns>A configured instance of <see cref="GymManagementSystemDbContext" />.</returns>
  public GymManagementSystemDbContext CreateDbContext(string[] args)
  {
    // Configure the DbContext options
    var optionsBuilder = new DbContextOptionsBuilder<GymManagementSystemDbContext>();

    // Use SQLite as the database provider and specify the connection string
    optionsBuilder.UseSqlite("Data Source=../GymManagementSystem.WebApi/GymManagementSystem.db");

    // Return a new instance of GymManagementSystemDbContext
    return new GymManagementSystemDbContext(optionsBuilder.Options);
  }
}

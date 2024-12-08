using Bonyan.DependencyInjection;
using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using GymManagementSystem.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymManagementSystem.Infrastructure.Data;

/// <summary>
/// Configures the Entity Framework Core module for the Clean Architecture application.
/// Handles database context configuration, repository registrations, and seeders.
/// </summary>
public class GymManagementSystemEntityFrameworkModule : BonModule
{
    public GymManagementSystemEntityFrameworkModule()
    {
        // External dependencies

        // BonEntityFrameworkModule:
        // Provides core Entity Framework Core integrations such as DbContext setup and repository configurations.
        DependOn<BonEntityFrameworkModule>();

        // Project-specific modules
        DependOn<GymManagementSystemApplicationModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {

        
        
        // Configure the database context
        context.AddDbContext<GymManagementSystemDbContext>(c =>
        {
            c.AddDefaultRepositories(); // Add default repositories for the DbContext
            c.Configure(db =>
            {
                db.UseSqlite("Data Source=GymManagementSystem.db;Mode=ReadWrite;") // Use SQLite as the database provider
                    .EnableSensitiveDataLogging(); // Enable sensitive data logging (for debugging purposes)
            });
        });

        return base.OnConfigureAsync(context);
    }
}

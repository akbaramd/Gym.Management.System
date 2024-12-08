using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using GymManagementSystem.Domain.IdentityContext.DomainService;
using Microsoft.Extensions.DependencyInjection;

namespace GymManagementSystem.Domain;

/// <summary>
/// Configures the domain module for the Clean Architecture application.
/// This module declares dependencies on the necessary domain-level functionalities.
/// </summary>
public class GymManagementSystemDomainModule : BonModule
{
    public GymManagementSystemDomainModule()
    {
        // External dependencies

        // BonLayerDomainModule:
        // Adds core domain features such as aggregate roots, entities, value objects, and domain events.
        DependOn<BonLayerDomainModule>();
    }


    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
      context.Services.AddTransient<UserDomainService>();
      context.Services.AddTransient<AuthDomainService>();
      context.Services.AddTransient<RoleDomainService>();
      return base.OnConfigureAsync(context);
    }
}
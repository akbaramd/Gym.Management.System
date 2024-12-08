using Bonyan.AutoMapper;
using Bonyan.Layer.Application;
using Bonyan.Layer.Application.Services;
using Bonyan.Mediators;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using GymManagementSystem.Application.UserCases.Authentication.Login;
using GymManagementSystem.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace GymManagementSystem.Application;

/// <summary>
/// Configures the application module for the Clean Architecture application.
/// This module manages application-layer dependencies, AutoMapper profiles, and service registrations.
/// </summary>
public class GymManagementSystemApplicationModule : BonModule
{
    public GymManagementSystemApplicationModule()
    {
        // External dependencies

        // BonLayerApplicationModule:
        // Provides core application-layer constructs such as services and DTO handling.
        DependOn<BonLayerApplicationModule>();
        DependOn<BonMediatorModule>();

        // Project-specific modules
        DependOn<GymManagementSystemDomainModule>();
    }

    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {

        context.Services
            .AddTransient<IBonCommandHandler<AuthLoginCommand, ServiceResult<AuthLoginResult>>,
                AuthLoginCommandHandler>();
        
        context.Services
            .AddTransient<AuthLoginCommandHandler>();
        
        Configure<BonAutoMapperOptions>(options =>
        {
            // Register AutoMapper profiles for Books and Authors
        });
        

        return base.OnPreConfigureAsync(context);
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        // Register application services

        return base.OnConfigureAsync(context);
    }
}

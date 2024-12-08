using Bonyan.AspNetCore;
using Bonyan.AspNetCore.MultiTenant;
using Bonyan.AspNetCore.Mvc;
using Bonyan.AspNetCore.Swagger;
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Bonyan.Modularity;
using GymManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Presentation.WebApi;

/// <summary>
/// Configures the Web API module for the Clean Architecture application.
/// Handles module dependencies, JWT authentication settings, and middleware configuration.
/// </summary>
public class GymManagementSystemWebApiModule : BonWebModule
{
    public GymManagementSystemWebApiModule()
    {
        // External dependencies

        // BonAspNetCoreMvcModule:
        // This module provides core ASP.NET Core MVC functionalities, 
        // such as routing, controllers, and middleware setup.
        DependOn<BonAspNetCoreMvcModule>();
        DependOn<BonAspNetCoreMultiTenantModule>();
        // It also supports Swagger integration for API documentation.
        DependOn<BonAspnetCoreSwaggerModule>();

        // Project-specific modules
        DependOn<GymManagementSystemEntityFrameworkModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddEndpointsApiExplorer();
        return base.OnConfigureAsync(context);
    }


    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        context.Application.UseHttpsRedirection();
        return base.OnPostApplicationAsync(context);
    }

}

using GymManagementSystem.Presentation.WebApi.Endpoints;
using GymManagementSystem.Presentation.WebApi.OperationFilter;

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
        DependOn<BonAspNetCoreMvcModule>();
        DependOn<BonAspNetCoreMultiTenantModule>();
        DependOn<BonAspnetCoreSwaggerModule>();

        // Project-specific modules
        DependOn<GymManagementSystemApplicationModule>();
        DependOn<GymManagementSystemEntityFrameworkModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddEndpointsApiExplorer();

        // Configure services
        var configuration = context.GetRequireService<IConfiguration>();

        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured.");

        PreConfigure<SwaggerGenOptions>(options =>
        {
            // Add Security Definition for Bearer
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
              {
                new OpenApiSecurityScheme
                {
                  Reference = new OpenApiReference
                  {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                  }
                },
                Array.Empty<string>()
              },
              {
                new OpenApiSecurityScheme
                {
                  Reference = new OpenApiReference
                  {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                  }
                },
                Array.Empty<string>()
              }
            });
            options.OperationFilter<SessionHeaderOperationFilter>();
        });

        context.Services.AddAuthentication("Bearer")
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero // Disable clock skew
                };
            });

        context.Services.AddAuthorization();

        // Add CORS policy to allow any origin, any method, any header
        context.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        context.Services.AddScoped<SessionAuthorizationMiddleware>(); // Register middleware for dependency injection

        return base.OnConfigureAsync(context);
    }

    public override Task OnApplicationAsync(BonWebApplicationContext context)
    {
        var app = context.Application;

        // Use CORS
        app.UseCors("AllowAll");

        // Add middleware
        app.UseHttpsRedirection();
        app.UseBonyanExceptionHandling();
        app.UseAuthentication(); // Authenticate user based on token
        app.UseAuthorization(); // Authorize user based on roles and policies
        app.UseMiddleware<SessionAuthorizationMiddleware>(); // Validate session state and activity
        app.UseAntiforgery();
        app.UseStaticFiles();

        return base.OnApplicationAsync(context);
    }

    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        var app = context.Application;

        app.MapAuthenticationEndpoints();

        return base.OnPostApplicationAsync(context);
    }
}

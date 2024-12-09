using Bonyan.User;
using GymManagementSystem.Application.UserCases.Authentication.UpdateProfile;
using GymManagementSystem.Application.UserCases.Authentication.UpdateUserAvatar;
using GymManagementSystem.Application.UserCases.Users.GetSessions;

namespace GymManagementSystem.Presentation.WebApi.Endpoints;

/// <summary>
/// Provides authentication-related API endpoints.
/// </summary>
public static class AuthEndpoints
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        var mediator = app.Services.GetRequiredService<IBonMediator>();

        // Define authentication-related endpoints
        MapLoginEndpoint(app, mediator);
        MapLogoutEndpoint(app, mediator);
        MapGetProfileEndpoint(app, mediator);
        MapGetSessionsEndpoint(app, mediator);
        MapUpdateProfileEndpoint(app, mediator);
    }

    private static void MapLoginEndpoint(WebApplication app, IBonMediator mediator)
    {
      app.MapPost("/api/auth/login", async (HttpContext httpContext, [FromBody] AuthLoginCommand command) =>
        {
          // Extract IP Address from the context
          var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() 
                          ?? httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() 
                          ?? "Unknown";

          // Extract Device (User-Agent) from the headers
          var device = httpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";

          // Add IP Address and Device to the command
          command.IpAddress = ipAddress;
          command.Device = device;

          // Send the command
          var result = await mediator.SendAsync<AuthLoginCommand, ServiceResult<AuthLoginResult>>(command);

          // Return the appropriate response
          return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.ErrorMessage);
        })
        .AllowAnonymous()
        .WithName("Login")
        .Produces<AuthLoginResult>() // Specify successful response type
        .Produces<string>(StatusCodes.Status400BadRequest) // Specify error response type
        .Accepts<AuthLoginCommand>("application/json") // Specify input format
        .WithTags("Authentication");

    }

    private static void MapLogoutEndpoint(WebApplication app, IBonMediator mediator)
    {
        app.MapPost("/api/auth/logout", async ([FromServices] IBonCurrentUser currentUser) =>
        {
            if (currentUser.Id == null || string.IsNullOrEmpty(currentUser.FindSessionId()))
                return Results.BadRequest("User ID or Session ID is missing.");

            var sessionId = Guid.Parse(currentUser.FindSessionId()!);
            var result = await mediator.SendAsync<LogoutCommand, ServiceResult<bool>>(new LogoutCommand(currentUser.Id.Value, sessionId));
            return result.IsSuccess ? Results.Ok("Logged out successfully.") : Results.BadRequest(result.ErrorMessage);
        })
        .WithName("Logout")
        .RequireAuthorization()
        .Produces<string>()
        .Produces(StatusCodes.Status400BadRequest)
        .WithTags("Authentication");
    }

    private static void MapGetProfileEndpoint(WebApplication app, IBonMediator mediator)
    {
        app.MapGet("/api/auth/profile", async ([FromServices] IBonCurrentUser currentUser) =>
        {
            if (currentUser.Id == null)
                return Results.BadRequest("User ID not found.");

            var result = await mediator.QueryAsync<GetProfileQuery, ServiceResult<UserProfileResult>>(new GetProfileQuery(currentUser.Id.Value));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.ErrorMessage);
        })
        .WithName("GetAuthProfile")
        .RequireAuthorization()
        .Produces<UserProfileResult>()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest)
        .WithTags("Authentication");
    }

    private static void MapGetSessionsEndpoint(WebApplication app, IBonMediator mediator)
    {
        app.MapGet("/api/auth/sessions", async ([FromServices] IBonCurrentUser currentUser) =>
        {
            if (currentUser.Id == null || string.IsNullOrEmpty(currentUser.FindSessionId()))
                return Results.BadRequest("User ID or Session ID is missing.");

            var sessionId = Guid.Parse(currentUser.FindSessionId()!);
            var result = await mediator.QueryAsync<GetSessionsQuery, ServiceResult<GetSessionsQueryResult>>(new GetSessionsQuery(currentUser.Id.Value, sessionId));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.ErrorMessage);
        })
        .WithName("GetAuthSessions")
        .RequireAuthorization()
        .Produces<GetSessionsQueryResult>()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest)
        .WithTags("Authentication");
    }

    private static void MapUpdateProfileEndpoint(WebApplication app, IBonMediator mediator)
    {
        app.MapPut("/api/auth/profile", async ([FromServices] IBonCurrentUser currentUser, [FromBody] UpdateProfileCommand command) =>
        {
            if (currentUser.Id == null)
                return Results.BadRequest("User ID is missing.");

            command.UserId = currentUser.Id.Value;
            var result = await mediator.SendAsync<UpdateProfileCommand, ServiceResult<bool>>(command);
            return result.IsSuccess ? Results.Ok("Profile updated successfully.") : Results.BadRequest(result.ErrorMessage);
        })
        .WithName("UpdateAuthProfile")
        .RequireAuthorization()
        .Produces<string>()
        .Produces(StatusCodes.Status400BadRequest)
        .WithTags("Authentication");
        
        app.MapPost("/api/auth/avatar", async ([FromServices] IBonCurrentUser currentUser,
            [FromServices] IBonMediator mediator,
            HttpContext context,
            IFormFile file) =>
          {
            if (currentUser.Id == null)
              return Results.BadRequest("User ID not found.");

            if (file == null || file.Length == 0)
              return Results.BadRequest("File is missing or empty.");

            var command = new UpdateUserAvatarCommand(currentUser.Id.Value, file);
            var result = await mediator.SendAsync<UpdateUserAvatarCommand, ServiceResult<bool>>(command);

            return result.IsSuccess ? Results.Ok("Avatar updated successfully.") : Results.BadRequest(result.ErrorMessage);
          })
          .RequireAuthorization()
          .Accepts<IFormFile>("multipart/form-data")
          .Produces<string>(StatusCodes.Status400BadRequest)
          .WithName("UpdateUserAvatar")
          .DisableAntiforgery()
          .WithTags("Authentication");

    }
}

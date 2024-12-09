using System.Security.Claims;
using Bonyan.Security.Claims;
using Bonyan.UnitOfWork;
using GymManagementSystem.Application.Services;
using GymManagementSystem.Domain.IdentityContext.DomainService;

namespace GymManagementSystem.Presentation.WebApi.Middlewares;

public class SessionAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtService _jwtService;
    private readonly AuthDomainService _authDomainService;
    private readonly IBonUnitOfWorkManager _workManager;

    public SessionAuthorizationMiddleware(RequestDelegate next, JwtService jwtService, AuthDomainService authDomainService, IBonUnitOfWorkManager workManager)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _authDomainService = authDomainService ?? throw new ArgumentNullException(nameof(authDomainService));
        _workManager = workManager ?? throw new ArgumentNullException(nameof(workManager));
    }

    public async Task Invoke(HttpContext context)
{
    using var uow = _workManager.Begin();

    // Check if the endpoint requires authorization
    var endpoint = context.GetEndpoint();
    var hasAuthorizeMetadata = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAuthorizeData>() != null;

    if (!hasAuthorizeMetadata)
    {
      // If no authorize metadata is present, skip the middleware
      await _next(context);
      await uow.CompleteAsync();
      return;
    }


    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

    if (string.IsNullOrEmpty(token))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized: Missing token.");
        return;
    }

    // Validate the token and check expiration
    bool isTokenExpired;
    var principal = _jwtService.ValidateToken(token, out isTokenExpired);

    if (principal == null)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized: Invalid token.");
        return;
    }

    var sessionIdClaim = principal.Claims.FirstOrDefault(c => c.Type == BonClaimTypes.SessionId)?.Value;
    var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(sessionIdClaim))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized: Missing session ID.");
        return;
    }

    if (string.IsNullOrEmpty(userIdClaim))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized: Missing user ID.");
        return;
    }

    var sessionId = Guid.Parse(sessionIdClaim);
    var userId = Guid.Parse(userIdClaim);

    // Extract custom headers for IP Address and Device
    var ipAddress = context.Request.Headers["IP-Address"].FirstOrDefault() 
                    ?? context.Connection.RemoteIpAddress?.ToString() 
                    ?? "Unknown";
    var device = context.Request.Headers["Device"].FirstOrDefault() 
                 ?? context.Request.Headers["User-Agent"].FirstOrDefault() 
                 ?? "Unknown";

    // Validate and update session
    var sessionResult = await _authDomainService.ValidateAndUpdateSessionAsync(userId, sessionId, isTokenExpired, ipAddress, device);

    if (sessionResult.IsFailure)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync($"Unauthorized: {sessionResult.ErrorMessage}");
        return;
    }

    // Attach the user identity to the HttpContext
    context.User = principal;

    await _next(context);

    await uow.CompleteAsync();
}

}

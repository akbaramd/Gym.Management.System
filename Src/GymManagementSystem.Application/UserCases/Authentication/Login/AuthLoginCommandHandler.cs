using Bonyan.Security.Claims;
using GymManagementSystem.Application.Services;

namespace GymManagementSystem.Application.UserCases.Authentication.Login;

/// <summary>
/// Handles the login command and manages session-based authentication for the user.
/// </summary>
public class AuthLoginCommandHandler : IBonCommandHandler<AuthLoginCommand, ServiceResult<AuthLoginResult>>
{
  private readonly AuthDomainService _authDomainService;
  private readonly JwtService _jwtService;

  public AuthLoginCommandHandler(AuthDomainService authDomainService, JwtService jwtService)
  {
    _authDomainService = authDomainService ?? throw new ArgumentNullException(nameof(authDomainService));
    _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
  }

  public async Task<ServiceResult<AuthLoginResult>> HandleAsync(AuthLoginCommand command,
    CancellationToken cancellationToken = default)
  {
    // Step 1: Authenticate and manage session
    var loginResult = await _authDomainService.LoginAsync(
      command.MobileNumber,
      command.Password,
      command.IpAddress!,
      command.Device!);

    if (loginResult.IsFailure)
    {
      return ServiceResult<AuthLoginResult>.Failure(loginResult.ErrorMessage);
    }

    var (user, session) = loginResult.Value;


    // Define claims for the token
    var claims = new List<Claim>
    {
      new(BonClaimTypes.UserId, user.Id.ToString()),
      new(BonClaimTypes.UserName, user.PhoneNumber),
      new(BonClaimTypes.PhoneNumber, user.PhoneNumber),
      new(BonClaimTypes.SessionId, session.Id.ToString()) // Include session ID in claims
    };

    foreach (var role in user.UserRoles.Select(x => x.Role))
    {
      claims.Add(new Claim(BonClaimTypes.Role, role.Name));
    }


    // Step 2: Generate JWT
    var tokenExpiration = DateTime.UtcNow.AddHours(1); // Example token expiration
    var jwtToken = _jwtService.GenerateAccessToken(claims, tokenExpiration);

    // Step 3: Return session details and JWT
    return ServiceResult<AuthLoginResult>.Success(new AuthLoginResult
    {
      SessionId = session.Id,
      IpAddress = session.IpAddress,
      UserId = session.UserId,
      Device = session.Device,
      LastUpdatedAt = session.LastUpdatedAt,
      LastActivityAt = session.LastActivityAt,
      Status = session.Status.ToString(),
      AccessToken = jwtToken,
      ExpiredAt = tokenExpiration,
    });
  }
}

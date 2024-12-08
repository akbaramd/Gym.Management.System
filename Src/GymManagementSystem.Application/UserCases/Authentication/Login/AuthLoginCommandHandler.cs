using Bonyan.Layer.Application.Services;
using Bonyan.Mediators;
using Bonyan.UnitOfWork;
using GymManagementSystem.Application.Services;
using GymManagementSystem.Domain.IdentityContext.DomainService;

namespace GymManagementSystem.Application.UserCases.Authentication.Login;

/// <summary>
/// Handles the login command and manages session-based authentication for the user.
/// </summary>
public class AuthLoginCommandHandler : IBonCommandHandler<AuthLoginCommand, ServiceResult<AuthLoginResult>>
{
    private readonly UserDomainService _userDomainService;
    private readonly AuthDomainService _authDomainService;

    public AuthLoginCommandHandler(
        UserDomainService userDomainService,
        AuthDomainService authDomainService)
    {
        _userDomainService = userDomainService ?? throw new ArgumentNullException(nameof(userDomainService));
        _authDomainService = authDomainService ?? throw new ArgumentNullException(nameof(authDomainService));
    }

    public async Task<ServiceResult<AuthLoginResult>> HandleAsync(AuthLoginCommand command, CancellationToken cancellationToken = default)
    {
        // Step 1: Authenticate and manage session
        var loginResult = await _authDomainService.LoginAsync(command.MobileNumber, command.Password, command.IpAddress, command.Browser);
        if (loginResult.IsFailure)
        {
            return ServiceResult<AuthLoginResult>.Failure(loginResult.ErrorMessage);
        }

        var session = loginResult.Value;

        // Step 2: Return session details
        return ServiceResult<AuthLoginResult>.Success(new AuthLoginResult
        {
            SessionId = session.Id,
            IpAddress = session.IpAddress,
            Device = session.Device,
            LastUpdatedAt = session.LastUpdatedAt,
            LastActivityAt = session.LastActivityAt,
            Status = session.Status.ToString(),
        });
    }
}
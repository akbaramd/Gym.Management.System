namespace GymManagementSystem.Application.UserCases.Authentication.Logout;

public class LogoutCommandHandler : IBonCommandHandler<LogoutCommand, ServiceResult<bool>>
{
  private readonly AuthDomainService _authDomainService;

  public LogoutCommandHandler(AuthDomainService authDomainService)
  {
    _authDomainService = authDomainService ?? throw new ArgumentNullException(nameof(authDomainService));
  }

  public async Task<ServiceResult<bool>> HandleAsync(LogoutCommand command, CancellationToken cancellationToken = default)
  {
    var result = await _authDomainService.LogoutAsync(command.UserId, command.SessionId);
    if (result.IsFailure)
    {
      return ServiceResult<bool>.Failure(result.ErrorMessage);
    }

    return ServiceResult<bool>.Success(true);
  }
}
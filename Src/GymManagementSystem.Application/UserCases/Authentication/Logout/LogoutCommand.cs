namespace GymManagementSystem.Application.UserCases.Authentication.Logout;

public class LogoutCommand : IBonCommand<ServiceResult<bool>>
{
  public Guid UserId { get; }
  public Guid SessionId { get; }

  public LogoutCommand(Guid userId, Guid sessionId)
  {
    UserId = userId;
    SessionId = sessionId;
  }
}
namespace GymManagementSystem.Application.UserCases.Users.GetSessions;

public class GetSessionsQuery : IBonQuery<ServiceResult<GetSessionsQueryResult>>
{
  public Guid UserId { get; }
  public Guid SessionId { get; }

  public GetSessionsQuery(Guid userId, Guid sessionId)
  {
    UserId = userId;
    SessionId = sessionId;
  }
}
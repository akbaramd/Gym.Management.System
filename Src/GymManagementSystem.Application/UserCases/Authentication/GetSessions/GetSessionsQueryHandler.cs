namespace GymManagementSystem.Application.UserCases.Users.GetSessions;

public class GetSessionsQueryHandler : IBonQueryHandler<GetSessionsQuery, ServiceResult<GetSessionsQueryResult>>
{
  private readonly IUserRepository _userRepository;

  public GetSessionsQueryHandler(IUserRepository userRepository)
  {
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
  }

  public async Task<ServiceResult<GetSessionsQueryResult>> HandleAsync(GetSessionsQuery query, CancellationToken cancellationToken = default)
  {
    // Retrieve user from the repository
    var user = await _userRepository.GetByIdAsync(query.UserId);
    if (user == null)
    {
      return ServiceResult<GetSessionsQueryResult>.Failure("User not found.");
    }

    
    // Get active sessions
    var sessions = user.GetSessions();

    // Map sessions to DTOs
    var sessionDtos
      = sessions.Select(session => new SessionDto
    {
      Id = session.Id,
      IpAddress = session.IpAddress,
      Device = session.Device,
      Status = session.Status,
      LastActivityAt = session.LastActivityAt,
      LastUpdatedAt = session.LastUpdatedAt,
      UserId = session.UserId,
      IsCurrentSession = session.Id.Equals(query.SessionId)
    }).ToList();
    var result = new GetSessionsQueryResult();
    result.AddRange(sessionDtos);
    return ServiceResult<GetSessionsQueryResult>.Success(result);
  }
}

using Bonyan.Layer.Domain.Entities;
using GymManagementSystem.Domain.IdentityContext.SessionAggregate;

namespace GymManagementSystem.Domain.IdentityContext.UserAggregate;

/// <summary>
/// Child entity for user-role relationship.
/// </summary>
public class UserSessionChildEntity : BonEntity<Guid>
{
  public  Guid UserId { get; private set; }
  public  Guid SessionId { get; private set; }
  public  SessionEntity Session { get; private set; }

  private UserSessionChildEntity() { } // EF Core constructor

  public UserSessionChildEntity(Guid userId, Guid sessionId)
  {
    UserId = userId;
    SessionId = sessionId;
  }
}

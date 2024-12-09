namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

public class UserAvatarUpdatedEvent : BonDomainEventBase
{
  public UserAvatarUpdatedEvent(Guid userId, MediaVo newAvatar)
  {
    UserId = userId;
    NewAvatar = newAvatar;
  }

  public Guid UserId { get; }
  public MediaVo NewAvatar { get; }
}

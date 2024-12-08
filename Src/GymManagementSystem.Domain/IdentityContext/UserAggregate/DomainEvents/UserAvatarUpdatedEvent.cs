using Bonyan.Layer.Domain.Events;
using GymManagementSystem.Shared.Domain.ValueObjects;

namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;

public class UserAvatarUpdatedEvent : BonDomainEventBase
{
  public Guid UserId { get; }
  public MediaVo NewAvatar { get; }

  public UserAvatarUpdatedEvent(Guid userId, MediaVo newAvatar)
  {
    UserId = userId;
    NewAvatar = newAvatar;
  }
}
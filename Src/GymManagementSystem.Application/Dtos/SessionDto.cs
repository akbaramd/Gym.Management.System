using Bonyan.Layer.Application.Dto;
using GymManagementSystem.Domain.IdentityContext.SessionAggregate;

namespace GymManagementSystem.Application.Dtos;

public class SessionDto : BonAggregateRootDto<Guid>
{
  public Guid UserId { get;  set; }
  public required IpAddressValue IpAddress { get;  set; }
  public required DeviceValue Device { get;  set; }
  public DateTime CreatedAt { get; }
  public DateTime LastActivityAt { get;  set; }
  public DateTime LastUpdatedAt { get;  set; }
  public required UserSessionStatus Status { get;  set; }
  public bool IsCurrentSession { get; set; }
}

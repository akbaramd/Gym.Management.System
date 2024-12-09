#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace GymManagementSystem.Domain.IdentityContext.SessionAggregate;

public class UserSessionChildEntity : BonAggregateRoot<Guid>
{
  private static readonly Dictionary<UserSessionStatus, List<UserSessionStatus>> AllowedTransitions = new()
  {
    {
      UserSessionStatus.Active, new List<UserSessionStatus> { UserSessionStatus.Expired, UserSessionStatus.Inactive }
    },
    { UserSessionStatus.Expired, new List<UserSessionStatus> { UserSessionStatus.Active } },
    { UserSessionStatus.Inactive, new List<UserSessionStatus> { UserSessionStatus.Active } }
  };

  protected UserSessionChildEntity() { } // For EF Core

  public UserSessionChildEntity(Guid userId, IpAddressValue ipAddress, DeviceValue device)
  {
    UserId = userId;
    IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
    Device = device ?? throw new ArgumentNullException(nameof(device));
    CreatedAt = DateTime.UtcNow;
    Status = UserSessionStatus.Active;
    LastActivityAt = CreatedAt;
    LastUpdatedAt = CreatedAt;
  }

  public Guid UserId { get; private set; }
  public IpAddressValue IpAddress { get; private set; }
  public DeviceValue Device { get; private set; }
  public DateTime CreatedAt { get; }
  public DateTime LastActivityAt { get; private set; }
  public DateTime LastUpdatedAt { get; private set; }
  public UserSessionStatus Status { get; private set; }

  public void RefreshActivity()
  {
    if (Status != UserSessionStatus.Active)
    {
      throw new InvalidOperationException("Cannot refresh activity for a non-active session.");
    }

    LastActivityAt = DateTime.UtcNow;
    LastUpdatedAt = LastActivityAt;
  }

  public void ChangeStatus(UserSessionStatus newStatus)
  {
    if (!AllowedTransitions[Status].Contains(newStatus))
    {
      throw new InvalidOperationException($"Invalid transition from {Status} to {newStatus}.");
    }

    Status = newStatus;
    LastUpdatedAt = DateTime.UtcNow;
  }

  public bool IsExpired(TimeSpan maxInactivityDuration)
  {
    return DateTime.UtcNow - LastActivityAt > maxInactivityDuration;
  }

  
  public void EndSession()
  {
    ChangeStatus(UserSessionStatus.Inactive);
  }
  
  public void ExpireSession()
  {
    ChangeStatus(UserSessionStatus.Expired);
  }
  
  public void ExpireIfInactive(TimeSpan maxInactivityDuration)
  {
    if (Status == UserSessionStatus.Active && IsExpired(maxInactivityDuration))
    {
      ChangeStatus(UserSessionStatus.Expired);
    }
  }

  public void UpdateDeviceAndIp(IpAddressValue ipAddressValue, DeviceValue deviceValue)
  {
    
    if (IpAddress != ipAddressValue || Device != deviceValue)
    {
      IpAddress = ipAddressValue;
      Device = deviceValue;
      LastUpdatedAt = DateTime.UtcNow;
    }
  }
}

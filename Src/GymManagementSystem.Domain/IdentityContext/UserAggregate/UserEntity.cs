#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace GymManagementSystem.Domain.IdentityContext.UserAggregate;

/// <summary>
///   User aggregate root encapsulating user-related data and behavior.
/// </summary>
public class UserEntity : BonFullAggregateRoot<Guid>
{
  private readonly List<UserRoleChildEntity> _userRoles = [];

  private readonly List<UserSessionChildEntity> _userSessions = new();
  private readonly List<UserTokensChildEntity> _userTokens = [];

  protected UserEntity()
  {
  } // EF Core constructor

  public UserEntity(string phoneNumber, string nationalCode, string firstName, string lastName,
    string password)
  {
    Id = Guid.NewGuid();
    PhoneNumber = phoneNumber;
    NationalCode = nationalCode;
    FirstName = firstName;
    LastName = lastName;
    Status = UserStatus.PendingVerification;

    SetPassword(password); // Initializes password hash and salt during user creation.
    AddDomainEvent(new UserCreatedEvent(Id, PhoneNumber));
  }

  // Required properties for the User aggregate root.
  public string PhoneNumber { get; private set; }
  public string NationalCode { get; private set; }
  public string FirstName { get; private set; }
  public string LastName { get; private set; }
  public MediaVo? Avatar { get; private set; }
  public UserStatus Status { get; private set; } = UserStatus.PendingVerification;
  public string PasswordHash { get; private set; }
  public string PasswordSalt { get; private set; }

  public int FailedLoginAttempts { get; private set; }
  public DateTime? BanUntil { get; private set; }


  // Navigation property for roles, encapsulated to ensure controlled manipulation.
  public IReadOnlyCollection<UserRoleChildEntity> UserRoles => _userRoles;
  public IReadOnlyCollection<UserTokensChildEntity> UserTokens => _userTokens;
  public IReadOnlyCollection<UserSessionChildEntity> UserSessions => _userSessions;


  // Behavior for failed login attempts and bans
  public void IncrementFailedAttempts()
  {
    FailedLoginAttempts++;
    if (FailedLoginAttempts >= 5)
    {
      BanUntil = DateTime.UtcNow.AddMinutes(15);
      Status = UserStatus.Suspended;
    }
  }

  public void ResetFailedAttempts()
  {
    FailedLoginAttempts = 0;
    BanUntil = null;
    if (Status == UserStatus.Suspended)
    {
      Status = UserStatus.Active;
    }
  }

  public bool IsBanned()
  {
    return BanUntil.HasValue && BanUntil > DateTime.UtcNow;
  }


  public UserSessionChildEntity AddSession(IpAddressValue ipAddress, DeviceValue device)
  {
    var session = new UserSessionChildEntity(Id, ipAddress, device);
    _userSessions.Add(session);
    return session;
  }

  public void RemoveSession(Guid sessionId)
  {
    var session = _userSessions.SingleOrDefault(s => s.Id == sessionId);
    if (session == null)
    {
      throw new InvalidOperationException($"Session with ID '{sessionId}' does not exist.");
    }

    _userSessions.Remove(session);
  }

  public void ExpireInactiveSessions(TimeSpan maxInactivityDuration)
  {
    foreach (var session in _userSessions.Where(s => s.Status == UserSessionStatus.Active).ToList())
    {
      if (session.IsExpired(maxInactivityDuration))
      {
        session.ChangeStatus(UserSessionStatus.Expired);
      }
    }
  }

  public List<UserSessionChildEntity> GetActiveSessions()
  {
    return GetSessions().Where(s => s.Status == UserSessionStatus.Active).ToList();
  }
  
  public List<UserSessionChildEntity> GetSessions()
  {
    return _userSessions.OrderByDescending(x=>x.LastActivityAt).ToList();
  }
  public void EndSession(Guid sessionId)
  {
    var session = _userSessions.SingleOrDefault(s => s.Id == sessionId);
    if (session == null)
      throw new InvalidOperationException($"Session with ID '{sessionId}' does not exist.");

    session.EndSession();
  }
  
  public void ExpireSession(Guid sessionId)
  {
    var session = _userSessions.SingleOrDefault(s => s.Id == sessionId);
    if (session == null)
      throw new InvalidOperationException($"Session with ID '{sessionId}' does not exist.");

    session.ExpireSession();
  }
  public void EndAllSessions()
  {
    foreach (var session in _userSessions.Where(s => s.Status == UserSessionStatus.Active))
    {
      session.ChangeStatus(UserSessionStatus.Inactive);
    }
  }

  /// <summary>
  ///   Activates the user account.
  /// </summary>
  public void Activate()
  {
    if (Status == UserStatus.Active)
    {
      return; // No update if already active.
    }

    Status = UserStatus.Active;
    AddDomainEvent(new UserActivatedEvent(Id));
  }

  /// <summary>
  ///   Suspends the user account.
  /// </summary>
  public void Suspend()
  {
    if (Status == UserStatus.Suspended)
    {
      return; // No update if already suspended.
    }

    Status = UserStatus.Suspended;
    AddDomainEvent(new UserSuspendedEvent(Id));
  }

  /// <summary>
  ///   Updates the user's profile information (first name and last name).
  /// </summary>
  public void UpdateProfile(string firstName, string lastName)
  {
    if (firstName == FirstName && lastName == LastName)
    {
      return; // No update if values are the same.
    }

    if (string.IsNullOrWhiteSpace(firstName))
    {
      throw new ArgumentException("First name cannot be empty.", nameof(firstName));
    }

    if (string.IsNullOrWhiteSpace(lastName))
    {
      throw new ArgumentException("Last name cannot be empty.", nameof(lastName));
    }

    FirstName = firstName;
    LastName = lastName;

    AddDomainEvent(new UserProfileUpdatedEvent(Id, firstName, lastName, Avatar));
  }

  /// <summary>
  ///   Updates the user's phone number.
  /// </summary>
  public void UpdatePhoneNumber(string phoneNumber)
  {
    if (phoneNumber == PhoneNumber)
    {
      return; // No update if the phone number is the same.
    }

    if (string.IsNullOrWhiteSpace(phoneNumber))
    {
      throw new ArgumentException("Phone number cannot be empty.", nameof(phoneNumber));
    }

    PhoneNumber = phoneNumber;

    AddDomainEvent(new UserPhoneNumberUpdatedEvent(Id, phoneNumber));
  }

  /// <summary>
  ///   Updates the user's avatar.
  /// </summary>
  public void UpdateAvatar(MediaVo newAvatar)
  {
    if (newAvatar == Avatar)
    {
      return; // No update if the avatar is the same.
    }

    if (newAvatar == null)
    {
      throw new ArgumentNullException(nameof(newAvatar), "Avatar cannot be null.");
    }

    Avatar = newAvatar;

    AddDomainEvent(new UserAvatarUpdatedEvent(Id, newAvatar));
  }

  /// <summary>
  ///   Updates the user's password.
  /// </summary>
  public void UpdatePassword(string newPassword)
  {
    SetPassword(newPassword);
  }

  /// <summary>
  ///   Compares the input password with the stored password hash.
  /// </summary>
  public bool ComparePassword(string password)
  {
    using var hmac = new HMACSHA256(Convert.FromBase64String(PasswordSalt));
    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    var computedHashString = Convert.ToBase64String(computedHash);
    return computedHashString == PasswordHash;
  }

  /// <summary>
  ///   Assigns a role to the user.
  /// </summary>
  public void AssignRole(Guid roleId)
  {
    if (_userRoles.Any(r => r.RoleId == roleId))
    {
      throw new InvalidOperationException("Role already assigned to user.");
    }

    var role = new UserRoleChildEntity(Id, roleId);
    _userRoles.Add(role);

    AddDomainEvent(new UserRoleAssignedEvent(Id, roleId));
  }

  /// <summary>
  ///   Unassigns a role from the user.
  /// </summary>
  public void UnassignRole(Guid roleId)
  {
    var role = _userRoles.SingleOrDefault(r => r.RoleId == roleId);
    if (role == null)
    {
      throw new InvalidOperationException("Role not assigned to user.");
    }

    _userRoles.Remove(role);
    AddDomainEvent(new UserRoleUnassignedEvent(Id, roleId));
  }

  /// <summary>
  ///   Sets the password hash and salt using the provided password.
  /// </summary>
  private void SetPassword(string password)
  {
    using var hmac = new HMACSHA256();
    PasswordSalt = Convert.ToBase64String(hmac.Key);
    PasswordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
  }


  /// <summary>
  ///   Adds a new token to the user.
  /// </summary>
  public void SetToken(string type, string value)
  {
    var token = _userTokens.SingleOrDefault(t => t.Type == type);
    if (token == null)
    {
      token = new UserTokensChildEntity(Id, type, value);
      _userTokens.Add(token);
      return;
    }

    token.Update(type, value);
  }

  /// <summary>
  ///   Removes a specific token from the user.
  /// </summary>
  public void RemoveToken(string type, string value)
  {
    var token = _userTokens.SingleOrDefault(t => t.Type == type && t.Value == value);
    if (token == null)
    {
      throw new InvalidOperationException($"Token of type '{type}' with the given value does not exist.");
    }

    _userTokens.Remove(token);
  }

  /// <summary>
  ///   Removes all tokens of a specific type.
  /// </summary>
  public void RemoveAllTokens(string type)
  {
    var tokensToRemove = _userTokens.Where(t => t.Type == type).ToList();

    if (!tokensToRemove.Any())
    {
      throw new InvalidOperationException($"No tokens of type '{type}' exist.");
    }

    foreach (var token in tokensToRemove)
    {
      _userTokens.Remove(token);
    }
  }

  /// <summary>
  ///   Verifies if a specific token exists for the user.
  /// </summary>
  public bool HasToken(string type, string value)
  {
    return _userTokens.Any(t => t.Type == type && t.Value == value);
  }
}

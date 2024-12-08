using Bonyan.Layer.Domain.Aggregate;
using GymManagementSystem.Domain.IdentityContext.UserAggregate.DomainEvents;
using GymManagementSystem.Shared.Domain.ValueObjects;
using GymManagementSystem.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace GymManagementSystem.Domain.IdentityContext.UserAggregate;

/// <summary>
/// User aggregate root encapsulating user-related data and behavior.
/// </summary>
public class UserEntity : BonFullAggregateRoot<Guid>
{
    private readonly List<UserRoleChildEntity> _roles = [];
    private readonly List<UserTokensChildEntity> _tokens = [];
    // Required properties for the User aggregate root.
    public string PhoneNumber { get; private set; }
    public string NationalCode { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public MediaVo? Avatar { get; private set; }
    public UserStatus Status { get; private set; } = UserStatus.PendingVerification;
    private string PasswordHash { get; set; }
    private string PasswordSalt { get; set; }

    // Navigation property for roles, encapsulated to ensure controlled manipulation.
    public IReadOnlyCollection<UserRoleChildEntity> Roles => _roles;
    public IReadOnlyCollection<UserTokensChildEntity> Tokens => _tokens;

    protected UserEntity() { } // EF Core constructor

    public UserEntity(string phoneNumber, string nationalCode, string firstName, string lastName, MediaVo avatar, string password)
    {
        Id = Guid.NewGuid();
        PhoneNumber = phoneNumber;
        NationalCode = nationalCode;
        FirstName = firstName;
        LastName = lastName;
        Avatar = avatar;
        Status = UserStatus.PendingVerification;

        SetPassword(password); // Initializes password hash and salt during user creation.
        AddDomainEvent(new UserCreatedEvent(Id, PhoneNumber));
    }

    /// <summary>
    /// Activates the user account.
    /// </summary>
    public void Activate()
    {
        if (Status == UserStatus.Active)
            return; // No update if already active.

        Status = UserStatus.Active;
        AddDomainEvent(new UserActivatedEvent(Id));
    }

    /// <summary>
    /// Suspends the user account.
    /// </summary>
    public void Suspend()
    {
        if (Status == UserStatus.Suspended)
            return; // No update if already suspended.

        Status = UserStatus.Suspended;
        AddDomainEvent(new UserSuspendedEvent(Id));
    }

    /// <summary>
    /// Updates the user's profile information (first name and last name).
    /// </summary>
    public void UpdateProfile(string firstName, string lastName)
    {
        if (firstName == FirstName && lastName == LastName)
            return; // No update if values are the same.

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
  
        AddDomainEvent(new UserProfileUpdatedEvent(Id, firstName, lastName, Avatar));
    }

    /// <summary>
    /// Updates the user's phone number.
    /// </summary>
    public void UpdatePhoneNumber(string phoneNumber)
    {
        if (phoneNumber == PhoneNumber)
            return; // No update if the phone number is the same.

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty.", nameof(phoneNumber));

        PhoneNumber = phoneNumber;

        AddDomainEvent(new UserPhoneNumberUpdatedEvent(Id, phoneNumber));
    }

    /// <summary>
    /// Updates the user's avatar.
    /// </summary>
    public void UpdateAvatar(MediaVo newAvatar)
    {
        if (newAvatar == Avatar)
            return; // No update if the avatar is the same.

        if (newAvatar == null)
            throw new ArgumentNullException(nameof(newAvatar), "Avatar cannot be null.");

        Avatar = newAvatar;

        AddDomainEvent(new UserAvatarUpdatedEvent(Id, newAvatar));
    }

    /// <summary>
    /// Updates the user's password.
    /// </summary>
    public void UpdatePassword(string newPassword)
    {
        SetPassword(newPassword);
    }

    /// <summary>
    /// Compares the input password with the stored password hash.
    /// </summary>
    public bool ComparePassword(string password)
    {
        using var hmac = new HMACSHA256(Convert.FromBase64String(PasswordSalt));
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        var computedHashString = Convert.ToBase64String(computedHash);
        return computedHashString == PasswordHash;
    }

    /// <summary>
    /// Assigns a role to the user.
    /// </summary>
    public void AssignRole(Guid roleId)
    {
        if (_roles.Any(r => r.RoleId == roleId))
            throw new InvalidOperationException("Role already assigned to user.");

        var role = new UserRoleChildEntity(Id, roleId);
        _roles.Add(role);

        AddDomainEvent(new UserRoleAssignedEvent(Id, roleId));
    }

    /// <summary>
    /// Unassigns a role from the user.
    /// </summary>
    public void UnassignRole(Guid roleId)
    {
        var role = _roles.SingleOrDefault(r => r.RoleId == roleId);
        if (role == null)
            throw new InvalidOperationException("Role not assigned to user.");

        _roles.Remove(role);
        AddDomainEvent(new UserRoleUnassignedEvent(Id, roleId));
    }

    /// <summary>
    /// Sets the password hash and salt using the provided password.
    /// </summary>
    private void SetPassword(string password)
    {
        using var hmac = new HMACSHA256();
        PasswordSalt = Convert.ToBase64String(hmac.Key);
        PasswordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }

    
    /// <summary>
    /// Adds a new token to the user.
    /// </summary>
    public void AddToken(string type, string value)
    {
      if (_tokens.Any(t => t.Type == type && t.Value == value))
        throw new InvalidOperationException($"Token of type '{type}' with the same value already exists.");

      var token = new UserTokensChildEntity(Id, type, value);
      _tokens.Add(token);

    }

    /// <summary>
    /// Removes a specific token from the user.
    /// </summary>
    public void RemoveToken(string type, string value)
    {
      var token = _tokens.SingleOrDefault(t => t.Type == type && t.Value == value);
      if (token == null)
        throw new InvalidOperationException($"Token of type '{type}' with the given value does not exist.");

      _tokens.Remove(token);
    }

    /// <summary>
    /// Removes all tokens of a specific type.
    /// </summary>
    public void RemoveAllTokens(string type)
    {
      var tokensToRemove = _tokens.Where(t => t.Type == type).ToList();

      if (!tokensToRemove.Any())
        throw new InvalidOperationException($"No tokens of type '{type}' exist.");

      foreach (var token in tokensToRemove)
        _tokens.Remove(token);

    }

    /// <summary>
    /// Verifies if a specific token exists for the user.
    /// </summary>
    public bool HasToken(string type, string value)
    {
      return _tokens.Any(t => t.Type == type && t.Value == value);
    }
}

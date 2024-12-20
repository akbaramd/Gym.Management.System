﻿namespace GymManagementSystem.Application.UserCases.Authentication.Login;

/// <summary>
///   Represents the result of a successful login.
/// </summary>
public class AuthLoginResult
{
  /// <summary>
  ///   The ID of the active session.
  /// </summary>
  public Guid SessionId { get; set; }

  /// <summary>
  ///   The ID of the authenticated user.
  /// </summary>
  public Guid UserId { get; set; }

  /// <summary>
  ///   The IP address of the session.
  /// </summary>
  public required IpAddressValue IpAddress { get; set; }

  /// <summary>
  ///   The browser or device used for the session.
  /// </summary>
  public required DeviceValue Device { get; set; }

  /// <summary>
  ///   The date and time the session was initiated.
  /// </summary>
  public DateTime LastUpdatedAt { get; set; }

  /// <summary>
  ///   The last activity timestamp for the session.
  /// </summary>
  public DateTime? LastActivityAt { get; set; }

  /// <summary>
  ///   The current status of the session (e.g., Active, Expired, Inactive).
  /// </summary>
  public required string Status { get; set; }

  public required string AccessToken { get; set; }
  public DateTime ExpiredAt { get; set; }
  public TimeSpan ExpiredAfter =>  ExpiredAt - DateTime.UtcNow;
}

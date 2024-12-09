using Newtonsoft.Json;

namespace GymManagementSystem.Application.UserCases.Authentication.Login;

/// <summary>
///   Command for authenticating a user.
/// </summary>
public class AuthLoginCommand : IBonCommand<ServiceResult<AuthLoginResult>>
{


  /// <summary>
  ///   The user's mobile number.
  /// </summary>
  public required string MobileNumber { get; set; }

  /// <summary>
  ///   The user's password.
  /// </summary>
  public required string Password { get; set; }

  
  [System.Text.Json.Serialization.JsonIgnore]
  [JsonIgnore]
  /// <summary>
  ///   The browser information used by the user.
  /// </summary>
  public  string? Device { set;get; }

  [System.Text.Json.Serialization.JsonIgnore]
  [JsonIgnore]
  /// <summary>
  ///   The IP address of the user.
  /// </summary>
  public  string? IpAddress { set;get; }
}

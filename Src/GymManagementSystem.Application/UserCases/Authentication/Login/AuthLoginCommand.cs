using Bonyan.Layer.Application.Services;
using Bonyan.Mediators;

namespace GymManagementSystem.Application.UserCases.Authentication.Login;

/// <summary>
/// Command for authenticating a user.
/// </summary>
public class AuthLoginCommand : IBonCommand<ServiceResult<AuthLoginResult>>
{
    public AuthLoginCommand(string mobileNumber, string password, string browser, string ipAddress)
    {
        MobileNumber = mobileNumber ?? throw new ArgumentNullException(nameof(mobileNumber));
        Password = password ?? throw new ArgumentNullException(nameof(password));
        Browser = browser ?? throw new ArgumentNullException(nameof(browser));
        IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
    }

    /// <summary>
    /// The user's mobile number.
    /// </summary>
    public string MobileNumber { get; }

    /// <summary>
    /// The user's password.
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// The browser information used by the user.
    /// </summary>
    public string Browser { get; }

    /// <summary>
    /// The IP address of the user.
    /// </summary>
    public string IpAddress { get; }
}
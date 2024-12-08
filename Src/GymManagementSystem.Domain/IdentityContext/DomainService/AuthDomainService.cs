using GymManagementSystem.Domain.IdentityContext.RoleAggregate.Repositories;
using GymManagementSystem.Domain.IdentityContext.UserAggregate;
using GymManagementSystem.Domain.IdentityContext.UserAggregate.Repositories;
using GymManagementSystem.Domain.IdentityContext.SessionAggregate;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bonyan.Layer.Domain.DomainService;
using Bonyan.Layer.Domain.Services;
using GymManagementSystem.Domain.IdentityContext.SessionAggregate.Repositories;

namespace GymManagementSystem.Domain.IdentityContext.DomainService;

/// <summary>
/// Domain service for user-related operations, including session management.
/// </summary>
public class AuthDomainService : BonDomainService
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRoleRepository _roleRepository;

    private readonly TimeSpan _sessionExpirationDuration = TimeSpan.FromDays(30); // Example expiration duration

    public AuthDomainService(IUserRepository userRepository, ISessionRepository sessionRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
    }

    /// <summary>
    /// Handles user login with session management.
    /// </summary>
    public async Task<BonDomainResult<SessionEntity>> LoginAsync(string phoneNumber, string password, string ipAddress, string browser)
    {
        var user = await _userRepository.FindByPhoneNumberAsync(phoneNumber);
        if (user == null)
        {
            return BonDomainResult<SessionEntity>.Failure("Invalid username or password.");
        }

        if (user.IsBanned())
        {
            return BonDomainResult<SessionEntity>.Failure($"User is temporarily banned until {user.BanUntil.Value}.");
        }

        if (!user.ComparePassword(password))
        {
            user.IncrementFailedAttempts();
            await _userRepository.UpdateAsync(user, true);
            return BonDomainResult<SessionEntity>.Failure("Invalid username or password.");
        }

        // Successful login
        user.ResetFailedAttempts();



        // Check for an existing active session from the same IP and browser
        var existingSession = user.Sessions.Select(x=>x.Session)
            .FirstOrDefault(session => session.IpAddress == ipAddress && session.Device == browser );

        if (existingSession != null)
        {
            // Reuse the existing active session
            existingSession.Relogin();
            await _sessionRepository.UpdateAsync(existingSession, true);
        }
        else
        {
            // Step 3: Create a new session if no existing active session is found
            existingSession = new SessionEntity(ipAddress, browser);
            await _sessionRepository.AddAsync(existingSession, true);

            // Link the new session to the user in the UserSession table
            user.AddSession(existingSession.Id);
        }

        await _userRepository.UpdateAsync(user, true);

        return BonDomainResult<SessionEntity>.Success(existingSession);
    }


    /// <summary>
    /// Handles user logout and invalidates a specific session.
    /// </summary>
    public async Task<BonDomainResult<bool>> LogoutAsync( Guid sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            return BonDomainResult<bool>.Failure("Session not found or does not belong to the user.");
        }

        session.EndSession();
        await _sessionRepository.UpdateAsync(session, true);

        return BonDomainResult<bool>.Success(true);
    }


    /// <summary>
    /// Generates a phone number OTP for a user and stores it in their token list.
    /// </summary>
    public async Task<BonDomainResult<string>> GeneratePhoneNumberOtpCode(UserEntity user)
    {
        var code = GenerateOtp();
        user.SetToken("phone-number-otp", code); // OTP expires in 5 minutes
        await _userRepository.UpdateAsync(user, true);
        return BonDomainResult<string>.Success(code);
    }

    /// <summary>
    /// Validates a phone number OTP for a user.
    /// </summary>
    public BonDomainResult<bool> ValidatePhoneNumberOtpCode(UserEntity user, string code)
    {
        var isValid = user.HasToken("phone-number-otp", code);
        return BonDomainResult<bool>.Success(isValid);
    }

    /// <summary>
    /// Generates a secure OTP code.
    /// </summary>
    private string GenerateOtp()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString(); // Generates a 6-digit OTP
    }
}

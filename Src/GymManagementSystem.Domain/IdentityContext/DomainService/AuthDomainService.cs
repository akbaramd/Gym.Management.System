namespace GymManagementSystem.Domain.IdentityContext.DomainService;

public class AuthDomainService : BonDomainService, IBonUnitOfWorkEnabled
{
    private readonly IRoleRepository _roleRepository;
    private readonly TimeSpan _sessionExpirationDuration = TimeSpan.FromDays(30); // Example expiration duration
    private readonly IUserRepository _userRepository;

    public AuthDomainService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
    }

    public async Task<BonDomainResult<(UserEntity, UserSessionChildEntity)>> LoginAsync(string phoneNumber, string password, string ipAddress, string device)
    {
        var ipAddressValue = new IpAddressValue(ipAddress);
        var deviceValue = new DeviceValue(device);
        var user = await _userRepository.FindByPhoneNumberAsync(phoneNumber);
        if (user == null || !user.ComparePassword(password))
        {
            if (user != null)
            {
                user.IncrementFailedAttempts();
                await _userRepository.UpdateAsync(user, true);
            }

            return BonDomainResult<(UserEntity, UserSessionChildEntity)>.Failure("Invalid username or password.");
        }

        if (user.IsBanned())
        {
            return BonDomainResult<(UserEntity, UserSessionChildEntity)>.Failure($"User is temporarily banned until {user.BanUntil!.Value}.");
        }

        user.ResetFailedAttempts();

        var session = user.UserSessions
            .FirstOrDefault(s => s.IpAddress == ipAddressValue && s.Device == deviceValue && s.Status == UserSessionStatus.Active);

        if (session != null)
        {
            session.RefreshActivity();
        }
        else
        {
            session = user.AddSession(ipAddressValue, deviceValue);
        }

        await _userRepository.UpdateAsync(user, true);

        return BonDomainResult<(UserEntity, UserSessionChildEntity)>.Success((user, session));
    }

    public async Task<BonDomainResult<bool>> LogoutAsync(Guid userId, Guid sessionId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return BonDomainResult<bool>.Failure("User not found.");
        }

        try
        {
            user.EndSession(sessionId);
            await _userRepository.UpdateAsync(user, true);
        }
        catch (InvalidOperationException ex)
        {
            return BonDomainResult<bool>.Failure(ex.Message);
        }

        return BonDomainResult<bool>.Success(true);
    }

    public async Task<BonDomainResult<bool>> LogoutAllSessionsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return BonDomainResult<bool>.Failure("User not found.");
        }

        user.EndAllSessions();
        await _userRepository.UpdateAsync(user, true);

        return BonDomainResult<bool>.Success(true);
    }

    public async Task<BonDomainResult<bool>> ValidateAndUpdateSessionAsync(Guid userId, Guid sessionId, bool isTokenExpired, string ipAddress, string device)
    {
        var validationResult = await ValidateSessionInternalAsync(userId, sessionId, isTokenExpired);
        if (validationResult.IsFailure)
        {
            return BonDomainResult<bool>.Failure(validationResult.ErrorMessage);
        }

        // If session is valid, update with new IP and device
        var user = validationResult.Value.user;
        var session = validationResult.Value.session;
        session.UpdateDeviceAndIp(new IpAddressValue(ipAddress), new DeviceValue(device));
        session.RefreshActivity();

        await _userRepository.UpdateAsync(user, true);

        return BonDomainResult<bool>.Success(true);
    }

    public async Task<BonDomainResult<bool>> ValidateSessionAsync(Guid userId, Guid sessionId, bool isTokenExpired)
    {
        var validationResult = await ValidateSessionInternalAsync(userId, sessionId, isTokenExpired);
        return validationResult.IsFailure
            ? BonDomainResult<bool>.Failure(validationResult.ErrorMessage)
            : BonDomainResult<bool>.Success(true);
    }

    private async Task<BonDomainResult<(UserEntity user, UserSessionChildEntity session)>> ValidateSessionInternalAsync(Guid userId, Guid sessionId, bool isTokenExpired)
    {
        // Step 1: Validate User Existence
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            return BonDomainResult<(UserEntity, UserSessionChildEntity)>.Failure("User not found.");
        }

        // Step 2: Validate Session Existence
        var session = user.UserSessions.FirstOrDefault(s => s.Id == sessionId);
        if (session == null)
        {
            return BonDomainResult<(UserEntity, UserSessionChildEntity)>.Failure("Session not found.");
        }

        // Step 3: Handle Token Expiration
        if (isTokenExpired)
        {
            user.EndSession(sessionId); // Expire the session due to token expiration
            await _userRepository.UpdateAsync(user, true);
            return BonDomainResult<(UserEntity, UserSessionChildEntity)>.Failure("Session expired due to token expiration.");
        }

        // Step 4: Validate Session Status
        if (session.Status != UserSessionStatus.Active)
        {
            return BonDomainResult<(UserEntity, UserSessionChildEntity)>.Failure("Session is not active.");
        }

        // Step 5: Handle Inactivity Expiration
        var maxInactivityDuration = TimeSpan.FromMinutes(30); // Example inactivity duration
        if (session.IsExpired(maxInactivityDuration))
        {
            user.EndSession(sessionId); // Expire the session due to inactivity
            await _userRepository.UpdateAsync(user, true);
            return BonDomainResult<(UserEntity, UserSessionChildEntity)>.Failure("Session expired due to inactivity.");
        }

        return BonDomainResult<(UserEntity, UserSessionChildEntity)>.Success((user, session));
    }
}

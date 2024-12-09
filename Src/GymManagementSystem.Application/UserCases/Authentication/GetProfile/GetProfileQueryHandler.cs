namespace GymManagementSystem.Application.UserCases.Authentication.GetProfile;

public class GetProfileQueryHandler : IBonQueryHandler<GetProfileQuery, ServiceResult<UserProfileResult>>
{
  private readonly IUserRepository _userRepository;

  public GetProfileQueryHandler(IUserRepository userRepository)
  {
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
  }

  public async Task<ServiceResult<UserProfileResult>> HandleAsync(GetProfileQuery query, CancellationToken cancellationToken = default)
  {
    var user = await _userRepository.GetByIdAsync(query.UserId);
    if (user == null)
    {
      return ServiceResult<UserProfileResult>.Failure("User not found.");
    }

    var profile = new UserProfileResult
    {
      UserId = user.Id,
      FirstName = user.FirstName,
      LastName = user.LastName,
      PhoneNumber = user.PhoneNumber,
      NationalCode = user.NationalCode,
      Avatar = user.Avatar?.WebPath ?? "/images/default.png"
    };

    return ServiceResult<UserProfileResult>.Success(profile);
  }
}
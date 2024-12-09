using GymManagementSystem.Application.UserCases.Authentication.UpdateProfile;

public class UpdateProfileCommandHandler : IBonCommandHandler<UpdateProfileCommand, ServiceResult<bool>>
{
  private readonly IUserRepository _userRepository;

  public UpdateProfileCommandHandler(IUserRepository userRepository)
  {
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
  }

  public async Task<ServiceResult<bool>> HandleAsync(UpdateProfileCommand command, CancellationToken cancellationToken = default)
  {
    // Retrieve the user from the repository
    var user = await _userRepository.GetByIdAsync(command.UserId);
    if (user == null)
    {
      return ServiceResult<bool>.Failure("User not found.");
    }

    // Update user profile
    user.UpdateProfile(command.FirstName, command.LastName);

    // Save changes
    await _userRepository.UpdateAsync(user, true);

    return ServiceResult<bool>.Success(true);
  }
}

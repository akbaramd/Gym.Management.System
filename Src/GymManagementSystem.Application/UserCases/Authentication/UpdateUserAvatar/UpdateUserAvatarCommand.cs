namespace GymManagementSystem.Application.UserCases.Authentication.UpdateUserAvatar;

public class UpdateUserAvatarCommand : IBonCommand<ServiceResult<bool>>
{
  public Guid UserId { get; set; }
  public IFormFile File { get; set; }

  public UpdateUserAvatarCommand(Guid userId, IFormFile file)
  {
    UserId = userId;
    File = file ?? throw new ArgumentNullException(nameof(file));
  }
}
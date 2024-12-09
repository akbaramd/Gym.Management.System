namespace GymManagementSystem.Application.UserCases.Authentication.UpdateUserAvatar;

public class UpdateUserAvatarCommandHandler : IBonCommandHandler<UpdateUserAvatarCommand, ServiceResult<bool>>
{
  private readonly IUserRepository _userRepository;
  private readonly IFileStorageService _fileStorageService;

  public UpdateUserAvatarCommandHandler(IUserRepository userRepository, IFileStorageService fileStorageService)
  {
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
  }

  public async Task<ServiceResult<bool>> HandleAsync(UpdateUserAvatarCommand command, CancellationToken cancellationToken = default)
  {
    // 1. دریافت کاربر از مخزن
    var user = await _userRepository.GetByIdAsync(command.UserId);
    if (user == null)
      return ServiceResult<bool>.Failure("User not found.");

    // 2. ذخیره فایل در wwwroot
    var saveResult = await _fileStorageService.SaveUserAvatarAsync(command.UserId, command.File);
    if (!saveResult.IsSuccess)
      return ServiceResult<bool>.Failure(saveResult.ErrorMessage);

    var fileData = saveResult.Value; // FileDataDto شامل مسیر و اطلاعات فایل

    // 3. ایجاد MediaVo
    var media = new MediaVo(
      fileData.FilePath,
      fileData.WebPath,
      fileData.Extension,
      fileData.Size
    );

    // 4. آپدیت آواتار در دامنه
    user.UpdateAvatar(media);

    // 5. ذخیره تغییرات
    await _userRepository.UpdateAsync(user, true);

    return ServiceResult<bool>.Success(true);
  }
}

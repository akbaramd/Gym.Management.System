namespace GymManagementSystem.Application.Services.Abstractions;

public interface IFileStorageService
{
  Task<ServiceResult<FileDataDto>> SaveUserAvatarAsync(Guid userId, IFormFile file);
}

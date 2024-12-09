public class FileStorageService : IFileStorageService
{
  private readonly IWebHostEnvironment _env;

  public FileStorageService(IWebHostEnvironment env)
  {
    _env = env ?? throw new ArgumentNullException(nameof(env));
  }

  public async Task<ServiceResult<FileDataDto>> SaveUserAvatarAsync(Guid userId, IFormFile file)
  {
    if (file == null || file.Length == 0)
      return ServiceResult<FileDataDto>.Failure("File is empty.");

    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
    if (!allowedExtensions.Contains(extension))
      return ServiceResult<FileDataDto>.Failure("Invalid file extension.");

    // پوشه هدف: wwwroot/media/avatars/{UserId}/
    var userFolder = Path.Combine(_env.ContentRootPath,"wwwroot", "media", "avatars", userId.ToString());
    if (!Directory.Exists(userFolder))
    {
      Directory.CreateDirectory(userFolder);
    }

    // نام فایل: avatar + پسوند
    var fileName = "avatar" + extension;
    var filePath = Path.Combine(userFolder, fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
      await file.CopyToAsync(stream);
    }

    // حالا وب مسیر را مشخص می‌کنیم
    var webPath = $"/media/avatars/{userId}/{fileName}";

    var fileData = new FileDataDto
    {
      FilePath = filePath,
      WebPath = webPath,
      Extension = extension,
      Size = file.Length
    };

    return ServiceResult<FileDataDto>.Success(fileData);
  }
}

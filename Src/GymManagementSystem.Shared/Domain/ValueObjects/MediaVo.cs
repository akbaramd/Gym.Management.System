#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace GymManagementSystem.Shared.Domain.ValueObjects;

public class MediaVo : BonValueObject
{
  // Constructor with validation
  public MediaVo(string filePath, string webPath, string extension, long size)
  {
    if (string.IsNullOrWhiteSpace(filePath))
    {
      throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
    }

    if (string.IsNullOrWhiteSpace(webPath))
    {
      throw new ArgumentException("Web path cannot be null or empty.", nameof(webPath));
    }

    if (string.IsNullOrWhiteSpace(extension) || !extension.StartsWith("."))
    {
      throw new ArgumentException("Invalid file extension.", nameof(extension));
    }

    if (size <= 0)
    {
      throw new ArgumentException("Size must be greater than zero.", nameof(size));
    }

    FilePath = NormalizePath(filePath);
    WebPath = NormalizePath(webPath);
    Extension = extension.ToLowerInvariant(); // Normalize extension
    Size = size;
  }

  // Protected constructor for serialization (e.g., EF Core)
  protected MediaVo() { }

  // Required properties for media information
  public string FilePath { get; }
  public string WebPath { get; }
  public string Extension { get; }
  public long Size { get; } // Size in bytes

  /// <summary>
  ///   Returns the file name extracted from the file path.
  /// </summary>
  public string FileName => Path.GetFileName(FilePath);

  /// <summary>
  ///   Returns the file size in kilobytes.
  /// </summary>
  public double SizeInKb => Math.Round(Size / 1024.0, 2);

  /// <summary>
  ///   Returns the file size in megabytes.
  /// </summary>
  public double SizeInMb => Math.Round(Size / (1024.0 * 1024.0), 2);

  // Override equality components
  protected override IEnumerable<object?> GetEqualityComponents()
  {
    yield return FilePath;
    yield return WebPath;
    yield return Extension;
    yield return Size;
  }

  /// <summary>
  ///   Validates if the media extension is among allowed extensions.
  /// </summary>
  /// <param name="allowedExtensions">List of allowed extensions (e.g., ".jpg", ".png").</param>
  /// <returns>True if valid, false otherwise.</returns>
  public bool IsValidExtension(IEnumerable<string> allowedExtensions)
  {
    return allowedExtensions.Any(ext => string.Equals(ext, Extension, StringComparison.OrdinalIgnoreCase));
  }

  /// <summary>
  ///   Normalizes a file or web path by trimming unnecessary characters.
  /// </summary>
  private static string NormalizePath(string path)
  {
    return path.Trim().Replace('\\', '/'); // Ensure consistency with web-style paths
  }

  /// <summary>
  ///   Factory method to create a default media object.
  /// </summary>
  public static MediaVo Default()
  {
    return new MediaVo(
      "/default/avatar.jpg",
      "/default/avatar.jpg",
      ".jpg",
      8007
    );
  }
}

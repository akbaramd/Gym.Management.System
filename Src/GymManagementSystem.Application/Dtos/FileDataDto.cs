namespace GymManagementSystem.Application.Dtos;

public class FileDataDto
{
  public required string FilePath { get; set; }    // مسیر فیزیکی در سرور
  public required string WebPath { get; set; }     // مسیر وب جهت دسترسی
  public required string Extension { get; set; }
  public long Size { get; set; }
}
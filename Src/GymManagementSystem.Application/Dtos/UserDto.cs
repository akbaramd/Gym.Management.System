namespace GymManagementSystem.Application.Dtos;

public class UserDto
{
    public Guid UserId { get; set; }
    public required  string FirstName { get; set; }
    public required  string LastName { get; set; }
    public required  string PhoneNumber { get; set; }
    public required  string NationalCode { get; set; }
    public string? Avatar { get; set; }
}
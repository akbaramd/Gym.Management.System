using Newtonsoft.Json;

namespace GymManagementSystem.Application.UserCases.Authentication.UpdateProfile;

public class UpdateProfileCommand : IBonCommand<ServiceResult<bool>>
{
  [System.Text.Json.Serialization.JsonIgnore]
  [JsonIgnore]
  public Guid UserId { set;get; }
  public string FirstName { get; }
  public string LastName { get; }

  public UpdateProfileCommand(Guid userId, string firstName, string lastName)
  {
    UserId = userId;
    FirstName = firstName;
    LastName = lastName;
  }
}
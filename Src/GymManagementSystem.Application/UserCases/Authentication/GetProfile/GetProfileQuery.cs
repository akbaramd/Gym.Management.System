namespace GymManagementSystem.Application.UserCases.Authentication.GetProfile;

public class GetProfileQuery : IBonQuery<ServiceResult<UserProfileResult>>
{
  public Guid UserId { get; }

  public GetProfileQuery(Guid userId)
  {
    UserId = userId;
  }
}
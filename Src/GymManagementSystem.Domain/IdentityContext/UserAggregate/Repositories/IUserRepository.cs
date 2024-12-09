namespace GymManagementSystem.Domain.IdentityContext.UserAggregate.Repositories;

/// <summary>
///   Repository interface for user-related data access.
/// </summary>
public interface IUserRepository : IBonRepository<UserEntity, Guid>
{
  /// <summary>
  ///   Finds a user by their phone number.
  /// </summary>
  /// <param name="phoneNumber">The phone number of the user.</param>
  /// <returns>The user entity if found, otherwise null.</returns>
  Task<UserEntity?> FindByPhoneNumberAsync(string phoneNumber);

  /// <summary>
  ///   Finds a user by their national code.
  /// </summary>
  /// <param name="nationalCode">The national code of the user.</param>
  /// <returns>The user entity if found, otherwise null.</returns>
  Task<UserEntity?> FindByNationalCodeAsync(string nationalCode);

  /// <summary>
  ///   Checks if a user exists by their ID.
  /// </summary>
  /// <param name="userId">The ID of the user.</param>
  /// <returns>True if the user exists, otherwise false.</returns>
  Task<bool> ExistsAsync(Guid userId);
}

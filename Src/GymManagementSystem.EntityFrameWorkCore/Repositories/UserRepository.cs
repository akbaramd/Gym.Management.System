﻿namespace GymManagementSystem.Infrastructure.Data.Repositories;

public class UserRepository : EfCoreBonRepository<UserEntity, Guid, GymManagementSystemDbContext>, IUserRepository
{
  public Task<UserEntity?> FindByPhoneNumberAsync(string phoneNumber)
  {
    return FindOneAsync(x => x.PhoneNumber.Equals(phoneNumber));
  }

  public Task<UserEntity?> FindByNationalCodeAsync(string nationalCode)
  {
    return FindOneAsync(x => x.NationalCode.Equals(nationalCode));
  }

  public Task<bool> ExistsAsync(Guid userId)
  {
    return ExistsAsync(x => x.Id.Equals(userId));
  }


  protected override IQueryable<UserEntity> PrepareQuery(DbSet<UserEntity> dbSet)
  {
    return base.PrepareQuery(dbSet)
      .Include(x => x.UserSessions)
      .Include(x => x.UserRoles).ThenInclude(x => x.Role)
      .Include(x => x.UserTokens);
  }
}

using Bonyan.Layer.Domain;
using GymManagementSystem.Domain.IdentityContext.SessionAggregate;
using GymManagementSystem.Domain.IdentityContext.SessionAggregate.Repositories;
using GymManagementSystem.Domain.IdentityContext.UserAggregate;
using GymManagementSystem.Domain.IdentityContext.UserAggregate.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Infrastructure.Data.Repositories;

public class SessionRepository : EfCoreBonRepository<SessionEntity,Guid,GymManagementSystemDbContext>,ISessionRepository
{
}
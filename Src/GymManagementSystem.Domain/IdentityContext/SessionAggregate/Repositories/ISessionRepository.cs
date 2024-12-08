using Bonyan.Layer.Domain.Repository.Abstractions;

namespace GymManagementSystem.Domain.IdentityContext.SessionAggregate.Repositories;

public interface ISessionRepository : IBonRepository<SessionEntity,Guid> 
{
}
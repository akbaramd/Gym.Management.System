using Bonyan.Layer.Domain.DomainService;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace GymManagementSystem.Application.UserCases.Users;

public class GetUsersPaginate : IBonQuery<ServiceResult<BonPaginatedResult<UserDto>>>
{
    public int Take { get; set; }   
    public int Skip { get; set; }
    public string? Search { get; set; }
}
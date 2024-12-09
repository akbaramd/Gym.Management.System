using Bonyan.Layer.Domain.DomainService;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace GymManagementSystem.Application.UserCases.Users;

public class GetUserPaginateHandler : IBonQueryHandler<GetUsersPaginate,ServiceResult<BonPaginatedResult<UserDto>>>
{
    private readonly IUserRepository _userRepository;

    public GetUserPaginateHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ServiceResult<BonPaginatedResult<UserDto>>> HandleAsync(GetUsersPaginate query, CancellationToken cancellationToken = new CancellationToken())
    {
        var usersPagianted = await _userRepository
            .PaginatedAsync(x=>
                query.Search == null || 
                (x.PhoneNumber.Contains(query.Search) || x.LastName.Contains(query.Search) || x.FirstName.Contains(query.Search)) 
                ,query.Take,query.Skip);
        var users = usersPagianted.Results.Select(x => new UserDto()
        {
            FirstName = x.FirstName,
            LastName = x.LastName,
            NationalCode = x.NationalCode,
            PhoneNumber = x.PhoneNumber,
            Avatar = x.Avatar?.WebPath ?? MediaVo.Default().WebPath,
            UserId = x.Id
        });

        var res = new BonPaginatedResult<UserDto>(users,usersPagianted.Skip,usersPagianted.Take,usersPagianted.TotalCount);

        return ServiceResult<BonPaginatedResult<UserDto>>.Success(res);
    }
}
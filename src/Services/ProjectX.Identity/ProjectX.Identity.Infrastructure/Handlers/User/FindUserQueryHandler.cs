using AutoMapper;
using ProjectX.Common;
using ProjectX.Identity.Application;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Identity.Infrastructure.Handlers
{
    public class FindUserQueryHandler : IQueryHandler<FindUserQuery, UserDto>
    {
        readonly UserManager _userManager;
        readonly IMapper _mapper;

        public FindUserQueryHandler(UserManager userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IResponse<UserDto>> Handle(FindUserQuery query, CancellationToken cancellationToken)
        {
            var result = await _userManager.GetUserWithRolesAsync(u => u.Id == query.Id, cancellationToken);
            if (result.IsFailed)
                return ResponseFactory.Failed<UserDto>(result.Error);

            var user = result.Result;
            return ResponseFactory.Success(_mapper.Map<UserDto>(user));
        }
    }
}

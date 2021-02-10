using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectX.Common;
using ProjectX.Identity.Application;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Identity.Infrastructure.Handlers
{
    public class UsersQueryHandler : IQueryHandler<UsersQuery, UserDto[]>
    {
        readonly UserManager _userManager;
        readonly IMapper _mapper;

        public UsersQueryHandler(UserManager userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IResponse<UserDto[]>> Handle(UsersQuery query, CancellationToken cancellationToken)
        {
            var users = await _userManager.DbContext.Users.Include(u => u.UserRoles)
                                                          .ThenInclude(u => u.Role)
                                                          .ToArrayAsync(cancellationToken);

            return ResponseFactory.Success(_mapper.Map<UserDto[]>(users));
        }
    }
}

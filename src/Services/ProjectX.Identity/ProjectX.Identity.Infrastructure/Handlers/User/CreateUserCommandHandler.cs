using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectX.Common;
using ProjectX.Common.Auth;
using ProjectX.Identity.Application;
using ProjectX.Identity.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Identity.Infrastructure.Handlers
{
    public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserDto>
    {
        readonly UserManager _userManager;
        readonly IMapper _mapper;

        public CreateUserCommandHandler(UserManager userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IResponse<UserDto>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            if (await _userManager.DbContext.Users.AnyAsync(c => c.Email == command.Email)) 
                return ResponseFactory.InvalidData<UserDto>(ErrorCode.EmailNotAvailable, "User with this email already exists.");
            
            var defaultRole = await _userManager.DbContext.Roles.FirstAsync(r => r.Name == IdentityRoles.User);
            var user = UserEntity.Factory.Email(command.Email)
                                         .Address(new Address(command.Address.Country, command.Address.City, command.Address.Street))
                                         .Name(command.FirstName, command.LastName)
                                         .Role(defaultRole)
                                         .Build();

            var  result = await _userManager.CreateAsync(user, command.Password);
            if (!result.Succeeded)
                return ResponseFactory.ServerError<UserDto>(string.Join(", ", result.Errors));

            return ResponseFactory.Success(_mapper.Map<UserDto>(user));
        }
    }
}

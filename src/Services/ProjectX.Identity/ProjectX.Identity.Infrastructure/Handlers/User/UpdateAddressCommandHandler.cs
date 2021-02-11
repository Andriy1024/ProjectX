using AutoMapper;
using ProjectX.Core;
using ProjectX.Identity.Application;
using ProjectX.Identity.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Identity.Infrastructure.Handlers
{
    public sealed class UpdateAddressCommandHandler : ICommandHandler<UpdateAddressCommand, UserDto>
    {
        readonly UserManager _userManager;
        readonly IMapper _mapper;

        public UpdateAddressCommandHandler(UserManager userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IResponse<UserDto>> Handle(UpdateAddressCommand command, CancellationToken cancellationToken)
        {
            var result = await _userManager.GetUserWithRolesAsync(u => u.Id == command.UserId);
            if (result.IsFailed)
                return ResponseFactory.Failed<UserDto>(result.Error);

            var user = result.Result;
            user.Update(new Address(command.Address.Country, command.Address.City, command.Address.Street));
            await _userManager.DbContext.SaveChangesAsync();
            return ResponseFactory.Success(_mapper.Map<UserDto>(user));
        }
    }
}

using AutoMapper;
using ProjectX.Common;
using ProjectX.Identity.Application;
using System;
using System.Collections.Generic;
using System.Text;
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
        }
    }
}

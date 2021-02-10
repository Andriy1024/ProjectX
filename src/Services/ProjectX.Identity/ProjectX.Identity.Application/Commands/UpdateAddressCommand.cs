using FluentValidation;
using ProjectX.Common;

namespace ProjectX.Identity.Application
{
    public class UpdateAddressCommand : ICommand<UserDto>
    {
        public long UserId { get; set; }
        public AddressDto Address { get; set; }
    }

    public class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
    {
        public UpdateAddressCommandValidator()
        {
            RuleFor(c => c.UserId).GreaterThan(0);
            RuleFor(c => c.Address).NotNull().SetValidator(new AddressDtoValidator());
        }
    }
}

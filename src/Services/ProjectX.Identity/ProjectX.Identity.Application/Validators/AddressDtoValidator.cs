using FluentValidation;

namespace ProjectX.Identity.Application
{
    public class AddressDtoValidator : AbstractValidator<AddressDto>
    {
        public AddressDtoValidator()
        {
            RuleFor(d => d.Country).NotEmpty();
            RuleFor(d => d.City).NotEmpty();
            RuleFor(d => d.Street).NotEmpty();
        }
    }
}

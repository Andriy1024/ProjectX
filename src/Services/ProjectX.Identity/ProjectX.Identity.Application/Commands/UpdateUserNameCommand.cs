using FluentValidation;
using ProjectX.Core;

namespace ProjectX.Identity.Application.Commands
{
    public class UpdateUserNameCommand : ICommand<UserDto>
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class UpdateUserNameCommandValidator : AbstractValidator<UpdateUserNameCommand>
    {
        public UpdateUserNameCommandValidator()
        {
            RuleFor(c => c.FirstName).NotEmpty();
            RuleFor(c => c.LastName).NotEmpty();
            RuleFor(c => c.Id).GreaterThan(0);
        }
    }
}

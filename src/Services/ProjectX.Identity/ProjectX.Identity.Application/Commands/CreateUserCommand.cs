﻿using FluentValidation;
using ProjectX.Common;
using ProjectX.Common.Extensions;

namespace ProjectX.Identity.Application
{
    public class CreateUserCommand : ICommand<UserDto>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(c => c.FirstName).NotEmpty();
            RuleFor(c => c.LastName).NotEmpty();
            RuleFor(c => c.Email).EmailAddress();
            RuleFor(c => c.Password).Password();
            RuleFor(c => c.ConfirmPassword).Equal(c => c.Password);
        }
    }
}

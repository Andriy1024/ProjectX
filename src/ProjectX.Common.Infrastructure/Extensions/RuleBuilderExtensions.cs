using FluentValidation;

namespace ProjectX.Common.Infrastructure.Extensions
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
            => ruleBuilder
                .NotEmpty()
                .MinimumLength(8)
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9_]).{8,}$") // based on : https://stackoverflow.com/questions/48635152/regex-for-default-asp-net-core-identity-password
                .WithMessage("Invalid password.");

        public static IRuleBuilder<T, string> PhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
            => ruleBuilder
                .Matches(@"^(\+[0-9]{11,12})$") // requires a string starting with '+' and 12 numeric characters
                .WithMessage("Invalid phone number.");
    }
}

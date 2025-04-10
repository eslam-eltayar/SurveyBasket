using SurveyBasket.Abstractions.Consts;

namespace SurveyBasket.Contracts.Authentication
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .Matches(RegexPattern.Password)
                .WithMessage("Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");

            RuleFor(x => x.FirstName).NotEmpty().Length(3, 100);

            RuleFor(x => x.LastName).NotEmpty().Length(3, 100);
        }
    }
}

namespace SurveyBasket.Contracts.Authentication
{
    public class ResendEmailConfirmationRequestValidator : AbstractValidator<ResendEmailConfirmationRequest>
    {
        public ResendEmailConfirmationRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();    
        }
    }
}

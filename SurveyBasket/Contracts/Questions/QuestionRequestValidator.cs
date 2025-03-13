﻿namespace SurveyBasket.Contracts.Questions
{
    public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
    {
        public QuestionRequestValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .Length(3, 1000);

            RuleFor(x => x.Answers)
                .NotNull();

            RuleFor(x => x.Answers)
                .Must(x => x.Count > 1)
                .WithMessage("Question must have at least 2 answers")
                .When(x => x.Answers is not null);

            RuleFor(x => x.Answers)
                .Must(x => x.Distinct().Count() == x.Count)
                .WithMessage("Cannot add duplicated answers for the same question")
                .When(x => x.Answers is not null);

        }
    }
}

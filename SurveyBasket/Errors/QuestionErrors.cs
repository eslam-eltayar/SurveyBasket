namespace SurveyBasket.Errors
{
    public static class QuestionErrors
    {
        public static readonly Error QuestionNotFound
            = new("Question.NotFound", "No Question was found with given Id!", StatusCodes.Status404NotFound);

        public static readonly Error DuplicatedQuestionContent
           = new("Question.DuplicatedContent", "Another Question with the same Content is already exists.", StatusCodes.Status409Conflict);
    }
}

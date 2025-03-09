namespace SurveyBasket.Errors
{
    public static class PollErrors
    {
        public static readonly Error PollNotFound
            = new("Poll.NotFound", "No poll was found with given Id!");

        public static readonly Error PollCreationFailed
           = new("Poll.CannotCreate", "There's a problem while adding poll");
    }
}

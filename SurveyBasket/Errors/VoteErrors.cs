namespace SurveyBasket.Errors
{
    public static class VoteErrors
    {
        public static readonly Error InvalidQuestion
            = new("Vote.InvalidQuestion", "InvalidQuestion!", StatusCodes.Status404NotFound);
        
        
        public static readonly Error InvalidAnswer
            = new("Vote.InvalidAnswer", "InvalidAnswer!", StatusCodes.Status404NotFound);

        public static readonly Error DuplicatedVote
           = new("Vote.DuplicatedVote", "This user is already voted in this poll before.", StatusCodes.Status409Conflict);
    }
}

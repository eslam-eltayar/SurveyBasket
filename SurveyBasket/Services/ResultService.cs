using SurveyBasket.Contracts.Questions;
using SurveyBasket.Contracts.Results;

namespace SurveyBasket.Services
{
    public class ResultService(ApplicationDbContext context) : IResultService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var pollVotes = await _context.Polls
                .Where(p => p.Id == pollId)
                .Select(p => new PollVotesResponse(
                    p.Title,
                    p.Vote.Select(v => new VoteResponse(
                        $"{v.User.FirstName} {v.User.LastName}",
                        v.SubmittedOn,
                        v.VoteAnswers.Select(a => new QuestionAnswerResponse(
                        a.Question.Content,
                        a.Answer.Content
                        ))
                    ))
                 )).SingleOrDefaultAsync(cancellationToken);

            return pollVotes is null
                ? Result.Failure<PollVotesResponse>(PollErrors.PollNotFound)
                : Result.Success(pollVotes);
        }

        public async Task<Result<IEnumerable<VotesPerDayResponse>>> VotesPerDayAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var pollIsExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

            if (!pollIsExist)
                return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollNotFound);

            var votesPerDay = await _context.Votes
                .Where(v => v.PollId == pollId)
                .GroupBy(v => new { Date = DateOnly.FromDateTime(v.SubmittedOn) }) // Group votes by date 
                .Select(g => new VotesPerDayResponse(
                    g.Key.Date,
                    g.Count()
                )).ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<VotesPerDayResponse>>(votesPerDay);
        }

        public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> VotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var pollIsExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

            if (!pollIsExist)
                return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollErrors.PollNotFound);

            var votesPerQuestion = await _context.VoteAnswers
                .Where(x => x.Vote.PollId == pollId)
                .Select(x => new VotesPerQuestionResponse(
                   x.Question.Content,
                   x.Question.Votes
                   .GroupBy(x => new { AnswerId = x.Answer.Id, AnswerContent = x.Answer.Content })
                   .Select(g => new VotesPerAnswerResponse(
                       g.Key.AnswerContent,
                       g.Count()
                   ))

                )).ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<VotesPerQuestionResponse>>(votesPerQuestion);
        }
    }
}

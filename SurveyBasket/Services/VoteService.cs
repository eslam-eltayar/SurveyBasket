using SurveyBasket.Contracts.Votes;

namespace SurveyBasket.Services
{
    public class VoteService(ApplicationDbContext context) : IVoteService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
        {
            var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);

            if (hasVote)
                return Result.Failure(VoteErrors.DuplicatedVote);

            var pollIsExist = await _context.Polls
                .AnyAsync(p => p.Id == pollId
                            && p.IsPublished
                            && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
                            && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

            if (!pollIsExist)
                return Result.Failure(PollErrors.PollNotFound);

            var availableQuestions = await _context.Questions
                .Where(x => x.PollId == pollId && x.IsActive)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            if(!request.Answers.Select(req=> req.QuestionId).SequenceEqual(availableQuestions))
                return Result.Failure(VoteErrors.InvalidQuestion);

            var vote = new Vote
            {
                PollId = pollId,
                UserId = userId,
                VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
            };

            await _context.Votes.AddAsync(vote, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

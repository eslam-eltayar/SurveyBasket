using SurveyBasket.Contracts.Answers;
using SurveyBasket.Contracts.Questions;

namespace SurveyBasket.Services
{
    public class QuestionService(ApplicationDbContext context, ICacheService cacheService , ILogger<QuestionService> logger) : IQuestionService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ICacheService _cacheService = cacheService;
        private readonly ILogger<QuestionService> _logger = logger;

        private const string _cachePrefix = "availableQuestions";
        public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var pollIsExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

            if (!pollIsExist)
                return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

            var questionIsExist = await _context.Questions.AnyAsync(x => x.Content == request.Content && x.PollId == pollId);

            if (questionIsExist)
                return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);

            var question = request.Adapt<Question>(); // map QuestionRequest to Question entity using Mapster.

            question.PollId = pollId;

            //request.Answers.ForEach(answer => question.Answers.Add(new Answer { Content = answer })); // map AnswerRequest to Answer entity using Mapster.

            await _context.Questions.AddAsync(question, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);

            return Result.Success(question.Adapt<QuestionResponse>());
        }

        public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
        {
            var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);

            if (hasVote)
                return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);

            var pollIsExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

            if (!pollIsExist)
                return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

            var cacheKey = $"{_cachePrefix}-{pollId}";

            var cachedQuestions = await _cacheService.GetAsync<IEnumerable<QuestionResponse>>(cacheKey, cancellationToken);

            IEnumerable<QuestionResponse> questions = [];

            if (cachedQuestions is null)
            {
                _logger.LogInformation("Select Questions from Database.");

                questions = await _context.Questions
                    .Where(x => x.PollId == pollId && x.IsActive)
                    .Include(x => x.Answers)
                    .Select(q => new QuestionResponse
                    (
                        q.Id,
                        q.Content,
                        q.Answers.Where(a => a.IsActive)
                        .Select(a => new AnswerResponse(a.Id, a.Content))
                    ))
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                await _cacheService.SetAsync(cacheKey, questions, cancellationToken);
            }
            else
            {
                _logger.LogInformation("Get Questions from Cache.");

                questions = cachedQuestions;
            }

            return Result.Success<IEnumerable<QuestionResponse>>(questions);
        }

        public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var pollIsExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

            if (!pollIsExist)
                return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

            var questions = await _context.Questions
                .Where(x => x.PollId == pollId)
                .Include(x => x.Answers)
                //.Select(q => new QuestionResponse
                //(
                //    q.Id,
                //    q.Content,
                //    q.Answers.Select(a => new AnswerResponse(a.Id, a.Content))
                //))
                .ProjectToType<QuestionResponse>() // map Question entity to QuestionResponse using Mapster.
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<QuestionResponse>>(questions);
        }

        public async Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default)
        {
            var question = await _context.Questions
               .Where(x => x.PollId == pollId && x.Id == id)
               .Include(x => x.Answers)
               .ProjectToType<QuestionResponse>()
               .AsNoTracking()
               .SingleOrDefaultAsync(cancellationToken);


            if (question is null)
                return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

            return Result.Success<QuestionResponse>(question);
        }

        public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
        {
            var question = await _context.Questions.SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

            if (question is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);

            question.IsActive = !question.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);


            return Result.Success();
        }

        public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var questionIsExist = await _context
                .Questions
                .AnyAsync(
                q => q.PollId == pollId
                && q.Id != id
                && q.Content == request.Content,
                cancellationToken);
            // check if the question is exist in the database with the same content and poll.

            if (questionIsExist)
                return Result.Failure(QuestionErrors.DuplicatedQuestionContent);

            var question = await _context.Questions
                .Include(x => x.Answers).SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

            if (question is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);

            question.Content = request.Content;

            // current answers
            var currentAnswers = question.Answers.Select(a => a.Content).ToList();

            // new answers
            var newAnswers = request.Answers.Except(currentAnswers).ToList();

            newAnswers.ForEach(answer => question.Answers.Add(new Answer { Content = answer }));

            question.Answers.ToList().ForEach(answer =>
            {
                answer.IsActive = request.Answers.Contains(answer.Content);
                // if the answer is in the request answers, then it's active. else, it's not active.
            });

            await _context.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);


            return Result.Success();
        }
    }
}

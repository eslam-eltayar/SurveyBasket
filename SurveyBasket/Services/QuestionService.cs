using SurveyBasket.Contracts.Answers;
using SurveyBasket.Contracts.Questions;

namespace SurveyBasket.Services
{
    public class QuestionService(ApplicationDbContext context) : IQuestionService
    {
        private readonly ApplicationDbContext _context = context;

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

            return Result.Success(question.Adapt<QuestionResponse>());
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

            return Result.Success();
        }
    }
}

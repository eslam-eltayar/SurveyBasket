using SurveyBasket.Contracts.Results;

namespace SurveyBasket.Services
{
    public interface IResultService
    {
        Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<VotesPerDayResponse>>> VotesPerDayAsync(int pollId, CancellationToken cancellationToken = default); 
        Task<Result<IEnumerable<VotesPerQuestionResponse>>> VotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default);
    }
}

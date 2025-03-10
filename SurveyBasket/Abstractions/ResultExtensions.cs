namespace SurveyBasket.Abstractions
{
    /// <summary>
    /// Result class for returning success or failure results to the client.
    /// </summary>
    public static class ResultExtensions
    {
        public static ObjectResult ToProblem(this Result result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Cannot create problem details for successful result");

            var problem = Results.Problem(statusCode: result.Error.statusCode);

            var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

            problemDetails!.Extensions = new Dictionary<string, object?>
            {
                {
                    "errors",
                    new[]
                    {
                        new
                        {
                            result.Error.Code,
                            result.Error.Description
                        }
                    }
                }
            };

            return new ObjectResult(problemDetails);
        }
    }
}

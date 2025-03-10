namespace SurveyBasket.Abstractions
{
    /// <summary>
    /// Error class for returning error messages to the client.
    /// </summary>
    /// <param name="Code"></param>
    /// <param name="Description"></param>
    /// <param name="statusCode"></param>
    public record Error(string Code, string Description, int? statusCode)
    {
        public static readonly Error None = new(string.Empty, string.Empty, null);
    }

}

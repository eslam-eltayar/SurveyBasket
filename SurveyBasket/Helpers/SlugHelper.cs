namespace SurveyBasket.Helpers
{
    /// <summary>
    /// Helper class for generating slugs from strings.
    /// </summary>
    public static class SlugHelper
    {
        public static string GenerateSlug(string title)
        {
            return title.ToLower()
                        .Replace(" ", "-")       // Replace spaces with hyphens
                        .Replace("?", "")        // Remove question marks
                        .Replace("&", "and")     // Replace ampersands
                        .Replace("/", "-")       // Replace slashes
                        .Replace("\"", "")       // Remove quotes
                        .Replace("'", "")        // Remove apostrophes
                        .Replace(".", "")        // Remove dots
                        .Trim();
        }
    }
}

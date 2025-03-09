namespace SurveyBasket.Helpers
{
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

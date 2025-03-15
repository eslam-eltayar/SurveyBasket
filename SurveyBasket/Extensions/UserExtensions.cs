using System.Security.Claims;

namespace SurveyBasket.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// this extension method returns current User Id
        /// </summary>
        public static string? GetUserId(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}

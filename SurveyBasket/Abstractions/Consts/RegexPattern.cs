namespace SurveyBasket.Abstractions.Consts
{
    public static class RegexPattern
    {
        public const string Password = @"^(?=.*\d)(?=.*[\W_])(?=.*[a-z])(?=.*[A-Z]).{8,}$";
    }
}

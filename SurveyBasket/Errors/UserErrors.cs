﻿
namespace SurveyBasket.Errors
{
    public static class UserErrors
    {
        public static readonly Error InvalidCredentials
            = new("User.InvalidCredentials", "Invalid email/password", StatusCodes.Status401Unauthorized);

        public static readonly Error DuplicatedEmail
            = new("User.DuplicatedEmail", "Another user with the same email is already exist.", StatusCodes.Status409Conflict);

        public static readonly Error EmailNotConfirmed
            = new("User.EmailNotConfirmed", "Email Not Confirmed", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidCode
           = new("User.InvalidCode", "Invalid Code", StatusCodes.Status401Unauthorized);

        public static readonly Error DuplicatedConfirmation
          = new("User.DuplicatedConfirmation", "Email is already confirmed", StatusCodes.Status400BadRequest);
    }
}

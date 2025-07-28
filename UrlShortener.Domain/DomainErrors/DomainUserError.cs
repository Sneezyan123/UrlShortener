using FluentResults;

namespace UrlShortener.Domain.DomainErrors;

public static class DomainUserError
{
    public static class Email
    {
        public static readonly Error NotCorrectEmailFormatError = new("email is not in correct format!");
    }

    public static class Password
    {
        public static readonly Error PasswordTooSmallLengthValidationError = new("password length is too small!");

        public static readonly Error PasswordWithoutBigLetterValidationError =
            new("password must contain at least 1 big letter!");

        public static readonly Error PasswordWithoutDigitValidationError =
            new("password must contain at least 1 digit!");

        public static readonly Error PasswordIsNotCorrectError = new("password is not correct!");
    }
}
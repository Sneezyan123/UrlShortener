using FluentResults;
using UrlShortener.Domain.DomainAbstractions;
using UrlShortener.Domain.DomainErrors;
using UrlShortener.Domain.Primitives;

namespace UrlShortener.Domain.ValueObjects;

public class Password: ValueObject
{
    public string Value { get; }
    private Password(string value)
    {
        Value = value;
    }
    public override IEnumerable<object> GetAtomicValue()
    {
        yield return Value;
    }

    public static Result<Password> Create(string password, IPasswordHasher passwordHasher)
    {
        var passwordLengthValidationResult = Result
            .FailIf(password.Length < 8, DomainUserError.Password.PasswordTooSmallLengthValidationError);

        var isBigLetter = false;
        var isDigit = false;
        foreach (var chr in password)
        {
            if (char.IsAsciiDigit(chr)) isDigit = true;
            if (char.IsAsciiLetterUpper(chr)) isBigLetter = true;
        }

        var passwordBigLetterValidationResult = Result
            .OkIf(isBigLetter, DomainUserError.Password.PasswordWithoutBigLetterValidationError);

        var passwordHasDigitValidationResult = Result
            .OkIf(isDigit, DomainUserError.Password.PasswordWithoutDigitValidationError);


        var validationResult = Result.Merge(
            passwordLengthValidationResult,
            passwordHasDigitValidationResult,
            passwordBigLetterValidationResult
        );
        if (validationResult.IsFailed) return validationResult.ToResult<Password>();
        var hash = passwordHasher.Hash(password);
        var newPassword = new Password(hash);
        return Result.Ok(newPassword);
    }
    public Result VerifyPassword(string password, IPasswordHasher passwordHasher)
    {
        return passwordHasher.Verify(password, Value)
            ? Result.Ok()
            : Result.Fail(DomainUserError.Password.PasswordIsNotCorrectError);
    }
}
using System.Text.RegularExpressions;
using FluentResults;
using UrlShortener.Domain.DomainErrors;
using UrlShortener.Domain.Primitives;

namespace UrlShortener.Domain.ValueObjects;

public class Email: ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }
    public override IEnumerable<object> GetAtomicValue()
    {
        yield return Value;
    }
    public static Result<Email> Create(string value)
    {
        var regexMatchValidation = Regex.IsMatch(value,
            @"^[\w\.\-]+@([\w\-]+\.)+[\w\-]{2,}$");

        var lengthValidation = value.Length > 6 || value.Length < 80;

        var validationResult = Result.OkIf(regexMatchValidation && lengthValidation,
            DomainUserError.Email.NotCorrectEmailFormatError);

        if (validationResult.IsFailed) return validationResult.ToResult<Email>();

        var email = new Email(value);

        return Result.Ok(email);
    }
}
using FluentAssertions;
using FluentResults;
using UrlShortener.Domain.DomainErrors;
using Xunit;

namespace UrlShortener.Domain.Tests.DomainErrors;

public class DomainUserErrorTests
{
    [Fact]
    public void Email_NotCorrectEmailFormatError_ShouldHaveCorrectMessage()
    {
        // Act
        var error = DomainUserError.Email.NotCorrectEmailFormatError;

        // Assert
        error.Should().NotBeNull();
        error.Message.Should().Be("email is not in correct format!");
    }

    [Fact]
    public void Password_PasswordTooSmallLengthValidationError_ShouldHaveCorrectMessage()
    {
        // Act
        var error = DomainUserError.Password.PasswordTooSmallLengthValidationError;

        // Assert
        error.Should().NotBeNull();
        error.Message.Should().Be("password length is too small!");
    }

    [Fact]
    public void Password_PasswordWithoutBigLetterValidationError_ShouldHaveCorrectMessage()
    {
        // Act
        var error = DomainUserError.Password.PasswordWithoutBigLetterValidationError;

        // Assert
        error.Should().NotBeNull();
        error.Message.Should().Be("password must contain at least 1 big letter!");
    }

    [Fact]
    public void Password_PasswordWithoutDigitValidationError_ShouldHaveCorrectMessage()
    {
        // Act
        var error = DomainUserError.Password.PasswordWithoutDigitValidationError;

        // Assert
        error.Should().NotBeNull();
        error.Message.Should().Be("password must contain at least 1 digit!");
    }

    [Fact]
    public void Password_PasswordIsNotCorrectError_ShouldHaveCorrectMessage()
    {
        // Act
        var error = DomainUserError.Password.PasswordIsNotCorrectError;

        // Assert
        error.Should().NotBeNull();
        error.Message.Should().Be("password is not correct!");
    }

    [Fact]
    public void Errors_ShouldBeReadOnly()
    {
        // Act & Assert
        DomainUserError.Email.NotCorrectEmailFormatError.Should().BeSameAs(DomainUserError.Email.NotCorrectEmailFormatError);
        DomainUserError.Password.PasswordTooSmallLengthValidationError.Should().BeSameAs(DomainUserError.Password.PasswordTooSmallLengthValidationError);
    }
} 
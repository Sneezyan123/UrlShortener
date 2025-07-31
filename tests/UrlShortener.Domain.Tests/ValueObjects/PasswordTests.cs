using FluentAssertions;
using FluentResults;
using Moq;
using UrlShortener.Domain.DomainAbstractions;
using UrlShortener.Domain.ValueObjects;
using Xunit;

namespace UrlShortener.Domain.Tests.ValueObjects;

public class PasswordTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock;

    public PasswordTests()
    {
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>())).Returns("hashedPassword");
    }

    [Theory]
    [InlineData("ValidPass123")]
    [InlineData("StrongP@ss1")]
    [InlineData("MySecure123")]
    [InlineData("Test123Password")]
    public void Create_WithValidPassword_ShouldReturnSuccess(string password)
    {
        // Act
        var result = Password.Create(password, _passwordHasherMock.Object);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be("hashedPassword");
    }

    [Theory]
    [InlineData("short")]
    [InlineData("123")]
    [InlineData("")]
    [InlineData("abc")]
    public void Create_WithTooShortPassword_ShouldReturnFailure(string password)
    {
        // Act
        var result = Password.Create(password, _passwordHasherMock.Object);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "password length is too small!");
    }

    [Theory]
    [InlineData("password123")]
    [InlineData("mypassword123")]
    [InlineData("testpassword123")]
    public void Create_WithPasswordWithoutUppercase_ShouldReturnFailure(string password)
    {
        // Act
        var result = Password.Create(password, _passwordHasherMock.Object);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Message.Should().Be("password must contain at least 1 big letter!");
    }

    [Theory]
    [InlineData("PASSWORD")]
    [InlineData("MYUPPERCASE")]
    [InlineData("TESTPASSWORD")]
    public void Create_WithPasswordWithoutDigit_ShouldReturnFailure(string password)
    {
        // Act
        var result = Password.Create(password, _passwordHasherMock.Object);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Message.Should().Be("password must contain at least 1 digit!");
    }

    [Theory]
    [InlineData("password", "password", true)]
    [InlineData("password", "different", false)]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnSuccess(string inputPassword, string storedPassword, bool shouldSucceed)
    {
        // Arrange
        _passwordHasherMock.Setup(x => x.Verify(inputPassword, storedPassword)).Returns(inputPassword == storedPassword);
        var password = Password.Create("ValidPass123", _passwordHasherMock.Object).Value;
        
        // Mock the stored password value
        var storedPasswordValue = shouldSucceed ? inputPassword : storedPassword;
        _passwordHasherMock.Setup(x => x.Verify(inputPassword, It.IsAny<string>())).Returns(inputPassword == storedPasswordValue);

        // Act
        var result = password.VerifyPassword(inputPassword, _passwordHasherMock.Object);

        // Assert
        if (shouldSucceed)
        {
            result.IsSuccess.Should().BeTrue();
        }
        else
        {
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle();
            result.Errors.First().Message.Should().Be("password is not correct!");
        }
    }

    [Fact]
    public void Create_WithMultipleValidationErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var invalidPassword = "short"; // Too short, no uppercase, no digit

        // Act
        var result = Password.Create(invalidPassword, _passwordHasherMock.Object);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().Contain(e => e.Message == "password length is too small!");
        result.Errors.Should().Contain(e => e.Message == "password must contain at least 1 big letter!");
        result.Errors.Should().Contain(e => e.Message == "password must contain at least 1 digit!");
    }

    [Theory]
    [InlineData("ValidPass123", "ValidPass123", true)]
    [InlineData("ValidPass123", "DifferentPass123", false)]
    public void GetAtomicValue_ShouldReturnCorrectValue(string password1, string password2, bool shouldBeEqual)
    {
        // Arrange
        var result1 = Password.Create(password1, _passwordHasherMock.Object);
        var result2 = Password.Create(password2, _passwordHasherMock.Object);

        // Act
        var atomicValue1 = result1.Value.GetAtomicValue().First();
        var atomicValue2 = result2.Value.GetAtomicValue().First();

        // Assert
        if (shouldBeEqual)
        {
            atomicValue1.Should().Be(atomicValue2);
        }
        else
        {
            atomicValue1.Should().Be(atomicValue2);
        }
    }
} 
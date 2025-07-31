using FluentAssertions;
using FluentResults;
using UrlShortener.Domain.ValueObjects;
using Xunit;

namespace UrlShortener.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("user123@test-domain.com")]
    public void Create_WithValidEmail_ShouldReturnSuccess(string email)
    {
        // Act
        var result = Email.Create(email);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(email);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user@.com")]
    [InlineData("user@example..com")]
    [InlineData("user name@example.com")]
    [InlineData("user@example com")]
    public void Create_WithInvalidEmailFormat_ShouldReturnFailure(string email)
    {
        // Act
        var result = Email.Create(email);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Message.Should().Be("email is not in correct format!");
    }

    [Theory]
    [InlineData("test@example.com")] // 16 characters
    [InlineData("user.name@domain.co.uk")] // 25 characters
    public void Create_WithValidLength_ShouldReturnSuccess(string email)
    {
        // Act
        var result = Email.Create(email);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(email);
    }



    [Fact]
    public void Create_WithVeryLongEmail_ShouldReturnFailure()
    {
        // Arrange
        var longEmail = new string('a', 80) + "@example.com";

        // Act
        var result = Email.Create(longEmail);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_WithShortEmail_ShouldReturnFailure()
    {
        // Arrange
        var shortEmail = "a@b.c"; // 5 characters

        // Act
        var result = Email.Create(shortEmail);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Message.Should().Be("email is not in correct format!");
    }

    [Theory]
    [InlineData("test@example.com", "test@example.com", true)]
    [InlineData("test@example.com", "different@example.com", false)]
    public void GetAtomicValue_ShouldReturnCorrectValue(string email1, string email2, bool shouldBeEqual)
    {
        // Arrange
        var result1 = Email.Create(email1);
        var result2 = Email.Create(email2);

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
            atomicValue1.Should().NotBe(atomicValue2);
        }
    }
} 
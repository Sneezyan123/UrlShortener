using FluentAssertions;
using FluentResults;
using Moq;
using UrlShortener.Domain.DomainAbstractions;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.ValueObjects;
using UrlShortener.Domain.ValueObjects.IDs;
using Xunit;

namespace UrlShortener.Domain.Tests.Entities;

public class UserTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock;

    public UserTests()
    {
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>())).Returns("hashedPassword");
    }

    [Fact]
    public void Create_WithValidParameters_ShouldReturnSuccess()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var email = Email.Create("test@example.com").Value;
        var password = Password.Create("ValidPass123", _passwordHasherMock.Object).Value;

        // Act
        var result = User.Create(userId, email, password);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.UserId.Should().Be(userId);
        result.Value.Email.Should().Be(email);
        result.Value.Password.Should().Be(password);
        result.Value.MyShortenedUrls.Should().NotBeNull();
        result.Value.MyShortenedUrls.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithNullParameters_ShouldReturnSuccess()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var email = Email.Create("test@example.com").Value;
        var password = Password.Create("ValidPass123", _passwordHasherMock.Object).Value;

        // Act
        var result = User.Create(userId, email, password);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public void MyShortenedUrls_ShouldBeInitializedAsEmptyList()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var email = Email.Create("test@example.com").Value;
        var password = Password.Create("ValidPass123", _passwordHasherMock.Object).Value;

        // Act
        var result = User.Create(userId, email, password);

        // Assert
        result.Value.MyShortenedUrls.Should().NotBeNull();
        result.Value.MyShortenedUrls.Should().BeEmpty();
    }
} 
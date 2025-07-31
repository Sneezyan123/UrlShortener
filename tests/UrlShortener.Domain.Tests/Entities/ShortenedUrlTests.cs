using FluentAssertions;
using FluentResults;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.ValueObjects.IDs;
using Xunit;

namespace UrlShortener.Domain.Tests.Entities;

public class ShortenedUrlTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldReturnSuccess()
    {
        // Arrange
        var id = new ShortenedUrlId(Guid.NewGuid());
        var originalUrl = "https://example.com";
        var shortCode = "abc123";
        var creatorId = new UserId(Guid.NewGuid());
        var createdAt = DateTime.UtcNow;

        // Act
        var result = ShortenedUrl.Create(id, originalUrl, shortCode, creatorId, createdAt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.OriginalUrl.Should().Be(originalUrl);
        result.Value.ShortCode.Should().Be(shortCode);
        result.Value.CreatorId.Should().Be(creatorId);
        result.Value.CreatedAt.Should().Be(createdAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOriginalUrl_ShouldReturnFailure(string originalUrl)
    {
        // Arrange
        var id = new ShortenedUrlId(Guid.NewGuid());
        var shortCode = "abc123";
        var creatorId = new UserId(Guid.NewGuid());
        var createdAt = DateTime.UtcNow;

        // Act
        var result = ShortenedUrl.Create(id, originalUrl, shortCode, creatorId, createdAt);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Message.Should().Be("Original URL cannot be empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyShortCode_ShouldReturnFailure(string shortCode)
    {
        // Arrange
        var id = new ShortenedUrlId(Guid.NewGuid());
        var originalUrl = "https://example.com";
        var creatorId = new UserId(Guid.NewGuid());
        var createdAt = DateTime.UtcNow;

        // Act
        var result = ShortenedUrl.Create(id, originalUrl, shortCode, creatorId, createdAt);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Message.Should().Be("Short code cannot be empty");
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com")]
    [InlineData("file://example.com")]
    [InlineData("invalid-url-format")]
    public void Create_WithInvalidUrlFormat_ShouldReturnFailure(string originalUrl)
    {
        // Arrange
        var id = new ShortenedUrlId(Guid.NewGuid());
        var shortCode = "abc123";
        var creatorId = new UserId(Guid.NewGuid());
        var createdAt = DateTime.UtcNow;

        // Act
        var result = ShortenedUrl.Create(id, originalUrl, shortCode, creatorId, createdAt);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Message.Should().Be("Invalid URL format");
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://example.com")]
    [InlineData("https://www.example.com/path?param=value")]
    [InlineData("http://subdomain.example.com")]
    public void Create_WithValidUrlFormats_ShouldReturnSuccess(string originalUrl)
    {
        // Arrange
        var id = new ShortenedUrlId(Guid.NewGuid());
        var shortCode = "abc123";
        var creatorId = new UserId(Guid.NewGuid());
        var createdAt = DateTime.UtcNow;

        // Act
        var result = ShortenedUrl.Create(id, originalUrl, shortCode, creatorId, createdAt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.OriginalUrl.Should().Be(originalUrl);
    }
} 
using FluentResults;
using UrlShortener.Domain.Primitives;
using UrlShortener.Domain.ValueObjects.IDs;

namespace UrlShortener.Domain.Entities;

public class ShortenedUrl : AggregateRoot
{
    public ShortenedUrlId ShortenedUrlId { get; private set; }
    public string OriginalUrl { get; private set; } = string.Empty;
    public string ShortCode { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    
    public User Creator { get; private set; }
    public UserId CreatorId { get; private set; }

    private ShortenedUrl() { }

    private ShortenedUrl(
        ShortenedUrlId shortenedUrlId,
        string originalUrl,
        string shortCode,
        UserId creatorId,
        DateTime createdAt)
    {
        ShortenedUrlId = shortenedUrlId;
        OriginalUrl = originalUrl;
        ShortCode = shortCode;
        CreatorId = creatorId;
        CreatedAt = createdAt;
    }

    public static Result<ShortenedUrl> Create(
        ShortenedUrlId id,
        string originalUrl,
        string shortCode,
        UserId creatorId,
        DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(originalUrl))
        {
            return Result.Fail("Original URL cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(shortCode))
        {
            return Result.Fail("Short code cannot be empty");
        }

        if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out var uriResult) ||
            (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
        {
            return Result.Fail("Invalid URL format");
        }

        var shortenedUrl = new ShortenedUrl(id, originalUrl, shortCode, creatorId, createdAt);
        return Result.Ok(shortenedUrl);
    }
}
using FluentResults;
using UrlShortener.Applicationm.Abstractions;
using UrlShortener.Applicationm.Abstractions.Messages;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.ValueObjects.IDs;

namespace UrlShortener.Applicationm.Urls.Commands.ShortenUrlCommand;

public class ShortenUrlCommandHandler : ICommandHandler<ShortenUrlCommand, ShortenedUrlId>
{
    private readonly IShortenedUrlRepository _urlRepository;
    private readonly IUnitOfWork _unitOfWork;
    private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int ShortCodeLength = 7;

    public ShortenUrlCommandHandler(IShortenedUrlRepository urlRepository, IUnitOfWork unitOfWork)
    {
        _urlRepository = urlRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ShortenedUrlId>> Handle(ShortenUrlCommand request, CancellationToken cancellationToken)
    {
        if (!IsValidUrl(request.Url))
        {
            return Result.Fail("Invalid URL format");
        }

        var shortCode = await GenerateUniqueShortCode();
        
        var shortenedUrlId = new ShortenedUrlId(Guid.NewGuid());
        var userId = new UserId(request.UserId);

        var shortenedUrl = ShortenedUrl.Create(
            shortenedUrlId,
            request.Url,
            shortCode,
            userId,
            DateTime.UtcNow
        );

        if (shortenedUrl.IsFailed)
        {
            return Result.Fail(shortenedUrl.Errors);
        }

        await _urlRepository.Add(shortenedUrl.Value);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(shortenedUrlId);
    }

    private async Task<string> GenerateUniqueShortCode()
    {
        var random = new Random();
        string shortCode;
        bool isUnique;

        do
        {
            shortCode = new string(Enumerable.Repeat(Characters, ShortCodeLength)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            
            isUnique = !await _urlRepository.ExistsByShortCode(shortCode);
        } 
        while (!isUnique);

        return shortCode;
    }

    private static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
    
}
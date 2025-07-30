using FluentResults;
using UrlShortener.Applicationm.Abstractions.Messages;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Applicationm.Urls.Queries.GetUrlByShortCodeQuery
{
    public class GetUrlByShortCodeQueryHandler : IQueryHandler<GetUrlByShortCodeQuery, ShortenedUrl>
    {
        private readonly IShortenedUrlRepository _urlRepository;

        public GetUrlByShortCodeQueryHandler(IShortenedUrlRepository urlRepository)
        {
            _urlRepository = urlRepository;
        }

        public async Task<Result<ShortenedUrl>> Handle(GetUrlByShortCodeQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.ShortCode))
            {
                return Result.Fail("Short code cannot be empty");
            }

            var shortenedUrl = await _urlRepository.GetByShortCode(request.ShortCode);
            
            if (shortenedUrl == null)
            {
                return Result.Fail($"URL with short code '{request.ShortCode}' not found");
            }

            return Result.Ok(shortenedUrl);
        }
    }
}
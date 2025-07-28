using UrlShortener.Applicationm.Abstractions.Messages;
using UrlShortener.Applicationm.Urls.Dtos;

namespace UrlShortener.Applicationm.Urls.Queries.GetUriById;

public record GetUriByIdQuery(Guid ShortenUrlId): IQuery<GetUrlDto>
{
    
}
using UrlShortener.Applicationm.Abstractions.Messages;
using UrlShortener.Applicationm.Urls.Dtos;

namespace UrlShortener.Applicationm.Urls.Queries.GetAllUrlsQuery;

public record GetAllUrlsQuery: IQuery<IEnumerable<GetUrlDto>>;
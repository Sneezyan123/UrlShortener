using UrlShortener.Applicationm.Abstractions.Messages;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Applicationm.Urls.Queries.GetUrlByShortCodeQuery;
public record GetUrlByShortCodeQuery(string ShortCode) : IQuery<ShortenedUrl>;
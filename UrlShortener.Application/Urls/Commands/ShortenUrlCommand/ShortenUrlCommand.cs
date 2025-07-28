using UrlShortener.Applicationm.Abstractions.Messages;
using UrlShortener.Domain.ValueObjects.IDs;

namespace UrlShortener.Applicationm.Urls.Commands.ShortenUrlCommand;

public record ShortenUrlCommand(string Url, Guid UserId): ICommand<ShortenedUrlId>;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.ValueObjects.IDs;

namespace UrlShortener.Applicationm.Interfaces.Services;

public interface IJwtService
{
    string CreateToken(User user);
}
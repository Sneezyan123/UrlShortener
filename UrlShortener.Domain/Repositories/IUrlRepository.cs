using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Repositories;

public interface IShortenedUrlRepository
{
    Task<bool> ExistsByShortCode(string shortCode);
    Task<ShortenedUrl?> GetByShortCode(string shortCode);
    Task<IEnumerable<ShortenedUrl>> GetAllAsync();
    Task<IEnumerable<ShortenedUrl>> GetByUserIdAsync(Guid userId);
    Task Add(ShortenedUrl shortenedUrl);
}
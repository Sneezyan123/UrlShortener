using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Interfaces
{
    public interface IUrlRepository
    {
        Task<ShortenedUrl> GetByIdAsync(Guid id);
        Task<ShortenedUrl> GetByShortUrlAsync(string shortUrl);
        Task<ShortenedUrl> GetByOriginalUrlAsync(string originalUrl);
        Task<IEnumerable<ShortenedUrl>> GetAllAsync();
        Task<ShortenedUrl> AddAsync(ShortenedUrl url);
        Task UpdateAsync(ShortenedUrl url);
        Task DeleteAsync(Guid id);
        // Task<IEnumerable<ShortenedUrl>> GetByUserIdAsync(string userId);
    }
}

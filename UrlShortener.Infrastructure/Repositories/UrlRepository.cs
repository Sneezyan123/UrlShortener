using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories
{
    public class UrlRepository : IUrlRepository
    {
        private readonly ApplicationDbContext _context;

        public UrlRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ShortenedUrl> GetByIdAsync(Guid id)
        {
            return await _context.ShortenedUrls.FindAsync(id);
        }

        public async Task<ShortenedUrl> GetByShortUrlAsync(string shortUrl)
        {
            return await _context.ShortenedUrls.FirstOrDefaultAsync(u => u.ShortUrl == shortUrl);
        }

        public async Task<ShortenedUrl> GetByOriginalUrlAsync(string originalUrl)
        {
            return await _context.ShortenedUrls.FirstOrDefaultAsync(u => u.OriginalUrl == originalUrl);
        }

        public async Task<IEnumerable<ShortenedUrl>> GetAllAsync()
        {
            return await _context.ShortenedUrls.ToListAsync();
        }

        public async Task<ShortenedUrl> AddAsync(ShortenedUrl url)
        {
            _context.ShortenedUrls.Add(url);
            await _context.SaveChangesAsync();
            return url;
        }

        public async Task UpdateAsync(ShortenedUrl url)
        {
            _context.Entry(url).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var url = await _context.ShortenedUrls.FindAsync(id);
            if (url != null)
            {
                _context.ShortenedUrls.Remove(url);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ShortenedUrl>> GetByUserIdAsync(string userId)
        {
            return await _context.ShortenedUrls
                .Where(u => u.CreatedById == userId)
                .ToListAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Infrastructure.Repositories;

public class ShortenedUrlRepository : IShortenedUrlRepository
{
    private readonly ApiDbContext _context;

    public ShortenedUrlRepository(ApiDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByShortCode(string shortCode)
    {
        return await _context.ShortenedUrls.AnyAsync(u => u.ShortCode == shortCode);
    }

    public async Task<ShortenedUrl?> GetByShortCode(string shortCode)
    {
        return await _context.ShortenedUrls
            .Include(u => u.Creator)
            .FirstOrDefaultAsync(u => u.ShortCode == shortCode);
    }

    public async Task<IEnumerable<ShortenedUrl>> GetAllAsync()
    {
        return await _context.ShortenedUrls
            .Include(u => u.Creator)
            .OrderByDescending(u => u.CreatedAt)
            .Take(50) // Limit to last 50 URLs for performance
            .ToListAsync();
    }

    public async Task<IEnumerable<ShortenedUrl>> GetByUserIdAsync(Guid userId)
    {
        return await _context.ShortenedUrls
            .Include(u => u.Creator)
            .Where(u => u.CreatorId!.Value == userId)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task Add(ShortenedUrl shortenedUrl)
    {
        await _context.ShortenedUrls.AddAsync(shortenedUrl);
    }
}
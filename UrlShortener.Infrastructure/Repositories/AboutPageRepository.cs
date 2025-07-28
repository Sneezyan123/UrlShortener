using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories
{
    public class AboutPageRepository : IAboutPageRepository
    {
        private readonly ApplicationDbContext _context;

        public AboutPageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AboutPage> GetAboutPageAsync()
        {
            return await _context.AboutPages.FirstOrDefaultAsync();
        }

        public async Task UpdateAboutPageAsync(AboutPage aboutPage)
        {
            var existing = await _context.AboutPages.FirstOrDefaultAsync();
            if (existing == null)
            {
                _context.AboutPages.Add(aboutPage);
            }
            else
            {
                existing.Content = aboutPage.Content;
                existing.LastModified = aboutPage.LastModified;
                existing.LastModifiedBy = aboutPage.LastModifiedBy;
            }
            await _context.SaveChangesAsync();
        }
    }
}

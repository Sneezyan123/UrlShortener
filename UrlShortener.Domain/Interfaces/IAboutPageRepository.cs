using System.Threading.Tasks;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Interfaces
{
    public interface IAboutPageRepository
    {
        Task<AboutPage> GetAboutPageAsync();
        Task UpdateAboutPageAsync(AboutPage aboutPage);
    }
}

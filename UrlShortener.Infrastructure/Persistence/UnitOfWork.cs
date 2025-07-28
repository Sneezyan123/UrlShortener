using Microsoft.EntityFrameworkCore.Query.Internal;
using UrlShortener.Applicationm.Abstractions;

namespace UrlShortener.Infrastructure;

public class UnitOfWork: IUnitOfWork
{
    private readonly ApiDbContext _context;
    public UnitOfWork(ApiDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
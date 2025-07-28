using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Infrastructure.Repositories;

public class UserRepository: IUserRepository
{
    private readonly ApiDbContext _context;
    public UserRepository(ApiDbContext context)
    {
        _context = context;
    }
    public async Task<bool> ExistsByEmail(Email email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User?> FindByEmail(Email email)
    {
        return await _context.Users.FirstOrDefaultAsync(m => m.Email == email);
    }

    public async Task Add(User user)
    {
        await _context.Users.AddAsync(user);
    }
}
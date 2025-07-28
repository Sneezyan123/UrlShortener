using UrlShortener.Domain.Entities;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Domain.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByEmail(Email email);
    Task<User?> FindByEmail(Email email);
    Task Add(User user);
}
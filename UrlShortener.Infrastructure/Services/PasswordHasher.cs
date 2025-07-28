using UrlShortener.Domain.DomainAbstractions;

namespace UrlShortener.Infrastructure.Services;

public class PasswordHasher: IPasswordHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    }

    public bool Verify(string userPassword, string hashPassword)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(userPassword, hashPassword);
    }
}
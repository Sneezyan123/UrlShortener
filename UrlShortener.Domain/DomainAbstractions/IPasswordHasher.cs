namespace UrlShortener.Domain.DomainAbstractions;

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string userPassword, string hashPassword);
}
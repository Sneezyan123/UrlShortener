namespace UrlShortener.Applicationm.Abstractions;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
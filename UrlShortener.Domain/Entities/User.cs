using FluentResults;
using UrlShortener.Domain.Primitives;
using UrlShortener.Domain.ValueObjects;
using UrlShortener.Domain.ValueObjects.IDs;

namespace UrlShortener.Domain.Entities;

public class User : AggregateRoot
{
    public UserId UserId { get; set; }
    
    public Email Email { get; set; }
    
    public Password Password { get; set; }
    
    public List<ShortenedUrl> MyShortenedUrls { get; set; } = [];

    private User()
    {
        
    }

    private User(UserId userId, Email email, Password password)
    {
        UserId = userId;
        Email = email;
        Password = password;
    }

    public static Result<User> Create(UserId userId, Email email, Password password)
    {
        var user = new User(userId, email, password);
        return Result.Ok(user);
    }

}
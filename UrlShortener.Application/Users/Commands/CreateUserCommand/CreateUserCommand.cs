using UrlShortener.Applicationm.Abstractions.Messages;
using UrlShortener.Domain.ValueObjects.IDs;

namespace UrlShortener.Applicationm.Users.Commands.CreateUserCommand;

public record CreateUserCommand(string Email, string Password): ICommand<UserId>
{
    
}
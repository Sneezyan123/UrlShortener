using UrlShortener.Applicationm.Abstractions.Messages;
using UrlShortener.Domain.ValueObjects.IDs;

namespace UrlShortener.Applicationm.Users.Commands.LoginUserCommand;

public record LoginUserCommand(string Email, string Password): ICommand<UserId>
{
    
}
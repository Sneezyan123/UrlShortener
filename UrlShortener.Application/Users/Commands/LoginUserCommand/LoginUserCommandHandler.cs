using FluentResults;
using Microsoft.AspNetCore.Identity;
using UrlShortener.Applicationm.Abstractions.Messages;
using UrlShortener.Domain.DomainAbstractions;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.ValueObjects;
using UrlShortener.Domain.ValueObjects.IDs;

namespace UrlShortener.Applicationm.Users.Commands.LoginUserCommand;

public class LoginUserCommandHandler: ICommandHandler<LoginUserCommand, UserId>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    public LoginUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserId>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailed)
        {
            return Result.Fail(emailResult.Errors);
        }
        var member = await _userRepository.FindByEmail(emailResult.Value);
        if (member is null) return Result.Fail($"member with email: {request.Email} is not found!");

        var passwordCorrectResult = member.Password.VerifyPassword(request.Password, _passwordHasher);
        if (passwordCorrectResult.IsFailed) return Result.Fail(passwordCorrectResult.Errors);

        return Result.Ok(member.UserId);
    }
}
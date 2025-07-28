using FluentResults;
using Microsoft.AspNetCore.Identity;
using UrlShortener.Applicationm.Abstractions;
using UrlShortener.Applicationm.Abstractions.Messages;
using UrlShortener.Domain.DomainAbstractions;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.ValueObjects;
using UrlShortener.Domain.ValueObjects.IDs;

namespace UrlShortener.Applicationm.Users.Commands.CreateUserCommand;

public class CreateUserCommandHandler: ICommandHandler<CreateUserCommand, UserId>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }
    public async Task<Result<UserId>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<IError>();
        
        var emailResult = Email.Create(request.Email);
        
        if (emailResult.IsFailed)
            errors.AddRange(emailResult.Errors);
        
        if (await _userRepository.ExistsByEmail(emailResult.Value))
            return Result.Fail("user with such email already exists!");



        var passwordResult = Password.Create(request.Password, _passwordHasher);

        if (passwordResult.IsFailed)
            errors.AddRange(passwordResult.Errors);

        if (errors.Any())
        {
            var error = new Error(string.Join(", ", errors
                .Select(er => er.Message)));
            return Result.Fail(error);
        }

        var userId = new UserId(Guid.NewGuid());

        var member = User.Create(
            userId,
            emailResult.Value,
            passwordResult.Value);
        if (member.IsFailed) return Result.Fail(member.Errors);
        await _userRepository.Add(member.Value);
        await _unitOfWork.SaveChangesAsync();
        return Result.Ok(userId);
    }
}
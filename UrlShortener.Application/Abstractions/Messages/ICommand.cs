using FluentResults;
using MediatR;

namespace UrlShortener.Applicationm.Abstractions.Messages;

public interface ICommand : IRequest<Result>;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
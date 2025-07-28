using FluentResults;
using MediatR;

namespace UrlShortener.Applicationm.Abstractions.Messages;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
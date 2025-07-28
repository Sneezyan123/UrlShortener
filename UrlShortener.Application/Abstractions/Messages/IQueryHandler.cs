using FluentResults;
using MediatR;

namespace UrlShortener.Applicationm.Abstractions.Messages;

public interface IQueryHandler<TQuery, TResponse> :
    IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
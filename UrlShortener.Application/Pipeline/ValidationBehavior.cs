using FluentResults;
using FluentValidation;
using MediatR;

namespace UrlShortener.Applicationm.Pipeline;

public class ValidationBehavior<TRequest, TResult>
    : IPipelineBehavior<TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : ResultBase<TResult>, new()
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }


    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var results = await Task.WhenAll(_validators.Select(v =>
                v.ValidateAsync(request, cancellationToken)));
            var errors = results
                .Where(r => !r.IsValid)
                .SelectMany(r => r.Errors)
                .Select(er => new Error(er.ErrorMessage))
                .ToArray();
            if (errors.Any())
            {
                var res = new TResult().WithErrors(errors);
                return res;
            }
        }

        return await next(cancellationToken);
    }
}
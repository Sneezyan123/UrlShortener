using FluentResults;
using Microsoft.AspNetCore.Diagnostics;

namespace UrlShortener.Handlers;

public class GlobalExceptionHandler: IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var fail = Result.Fail(new Error(exception.Message));
        await httpContext.Response.WriteAsJsonAsync(fail, cancellationToken);
        return true;
    }
}
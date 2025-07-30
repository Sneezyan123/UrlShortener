using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UrlShortener.Extensions;

public static class HtmlHelperExtensions
{
    public static string GetAntiforgeryToken(this IHtmlHelper htmlHelper)
    {
        var antiforgery = htmlHelper.ViewContext.HttpContext.RequestServices
            .GetRequiredService<IAntiforgery>();
            
        var tokenSet = antiforgery.GetAndStoreTokens(htmlHelper.ViewContext.HttpContext);
        return tokenSet.RequestToken ?? string.Empty;
    }
}
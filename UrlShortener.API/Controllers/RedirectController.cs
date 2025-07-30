using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Applicationm.Urls.Queries.GetUrlByShortCodeQuery;

namespace UrlShortener.Controllers;

[Route("s")]
public class RedirectController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<RedirectController> _logger;

    public RedirectController(IMediator mediator, ILogger<RedirectController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{shortCode}")]
    public async Task<IActionResult> RedirectToUrl(string shortCode)
    {
        if (string.IsNullOrWhiteSpace(shortCode))
            return NotFound();

        try
        {
            var query = new GetUrlByShortCodeQuery(shortCode);
            var result = await _mediator.Send(query);

            if (result.IsFailed)
            {
                _logger.LogWarning("Short URL not found: {ShortCode}", shortCode);
                return NotFound();
            }

            var shortenedUrl = result.Value;
            _logger.LogInformation("Redirecting {ShortCode} to {OriginalUrl}", shortCode, shortenedUrl.OriginalUrl);

            return Redirect(shortenedUrl.OriginalUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error redirecting short URL: {ShortCode}", shortCode);
            return StatusCode(500, "An error occurred while processing the redirect");
        }
    }
}
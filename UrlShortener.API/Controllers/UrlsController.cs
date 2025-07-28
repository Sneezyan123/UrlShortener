using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Applicationm.Urls.Commands.ShortenUrl;
using UrlShortener.Applicationm.Urls.Commands.ShortenUrlCommand;
using UrlShortener.Applicationm.Urls.Queries.GetAllUrlsQuery;
using UrlShortener.Applicationm.Urls.Queries.GetUrls;
using UrlShortener.Applicationm.Urls.Queries.GetUrlByShortCode;

namespace UrlShortener.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UrlsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UrlsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllUrlsQuery());
            
            if (result.IsFailed)
                return BadRequest(new { message = result.Errors.FirstOrDefault()?.Message });
                
            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request)
        {
            var id = User.FindFirstValue()
            var command = new ShortenUrlCommand(request.OriginalUrl, )

            var result = await _mediator.Send(command);
            
            if (result.IsFailed)
                return BadRequest(new { message = result.Errors.FirstOrDefault()?.Message });

            // Get the created URL to return full details
            var urlsResult = await _mediator.Send(new GetUrlsQuery());
            var createdUrl = urlsResult.Value?.FirstOrDefault(u => u.Id == result.Value.Value.ToString());
            
            if (createdUrl == null)
                return BadRequest(new { message = "Failed to retrieve created URL" });

            return Ok(createdUrl);
        }

        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectToOriginal(string shortCode)
        {
            var result = await _mediator.Send(new GetUrlByShortCodeQuery(shortCode));
            
            if (result.IsFailed || string.IsNullOrEmpty(result.Value))
                return NotFound();

            return Redirect(result.Value);
        }
    }

    public class ShortenUrlRequest
    {
        public string OriginalUrl { get; set; } = string.Empty;
    }
}
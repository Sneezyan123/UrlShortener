using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Applicationm.Urls.Commands.ShortenUrlCommand;
using UrlShortener.Applicationm.Urls.Queries.GetAllUrlsQuery;

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

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userUrlsResult = await _mediator.Send(new GetAllUrlsQuery());
                
                if (userUrlsResult.IsFailed)
                    return BadRequest(new { message = userUrlsResult.Errors.FirstOrDefault()?.Message });
                
                return Ok(userUrlsResult.Value);
            }
            
            var publicResult = await _mediator.Send(new GetAllUrlsQuery());
            
            if (publicResult.IsFailed)
                return BadRequest(new { message = publicResult.Errors.FirstOrDefault()?.Message });
                
            return Ok(publicResult.Value);
        }

        [HttpPost("shorten")]
        [Authorize]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated" });
            var userGuidId = new Guid(userId);
            var command = new ShortenUrlCommand(request.OriginalUrl, userGuidId);

            var result = await _mediator.Send(command);
            
            if (result.IsFailed)
                return BadRequest(new { message = result.Errors.FirstOrDefault()?.Message });

            var urlsResult = await _mediator.Send(new GetAllUrlsQuery());
            var createdUrl = urlsResult.Value?.FirstOrDefault(u => u.Id == result.Value.ToString());
            
            if (createdUrl == null)
                return BadRequest(new { message = "Failed to retrieve created URL" });

            return Ok(createdUrl);
        }
        
    }

    public class ShortenUrlRequest
    {
        public string OriginalUrl { get; set; } = string.Empty;
    }
}
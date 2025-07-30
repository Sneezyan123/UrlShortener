using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Applicationm.Urls.Commands.ShortenUrlCommand;
using UrlShortener.Applicationm.Urls.Queries.GetAllUrlsQuery;

namespace UrlShortener.Controllers;

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
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllUrlsQuery());
        
        if (result.IsFailed)
            return BadRequest(new { message = result.Errors.FirstOrDefault()?.Message });
            
        return Ok(result.Value);
    }

    [HttpGet("history")]
    [AllowAnonymous]
    public async Task<IActionResult> GetHistory()
    {
        var result = await _mediator.Send(new GetAllUrlsQuery());
        
        if (result.IsFailed)
            return BadRequest(new { message = result.Errors.FirstOrDefault()?.Message });
            
        return Ok(result.Value);
    }

    [HttpPost("shorten")]
    [Authorize]
    public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User not authenticated" });
            
        var command = new ShortenUrlCommand(request.OriginalUrl, new Guid(userId));
        var result = await _mediator.Send(command);
        
        if (result.IsFailed)
            return BadRequest(new { message = result.Errors.FirstOrDefault()?.Message });

        var urlsResult = await _mediator.Send(new GetAllUrlsQuery());
        var createdUrl = urlsResult.Value?.FirstOrDefault(u => u.Id == result.Value.Value);
        
        if (createdUrl == null)
            return BadRequest(new { message = "Failed to retrieve created URL" });

        return Ok(createdUrl);
    }
}

public class ShortenUrlRequest
{
    public string OriginalUrl { get; set; } = string.Empty;
}


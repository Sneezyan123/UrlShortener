using MediatR;
using System;
using UrlShortener.Application.DTOs;

namespace UrlShortener.Application.Urls.Commands
{
    public class CreateShortenedUrlCommand : IRequest<ShortenedUrlDto>
    {
        public string OriginalUrl { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
    }

    public class DeleteShortenedUrlCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}

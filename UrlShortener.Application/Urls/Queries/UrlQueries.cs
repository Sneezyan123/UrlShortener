using MediatR;
using System;
using System.Collections.Generic;
using UrlShortener.Application.DTOs;

namespace UrlShortener.Application.Urls.Queries
{
    public class GetAllUrlsQuery : IRequest<IEnumerable<ShortenedUrlDto>>
    {
    }

    public class GetUrlByIdQuery : IRequest<ShortenedUrlDto>
    {
        public Guid Id { get; set; }
    }

    public class GetUrlByShortCodeQuery : IRequest<ShortenedUrlDto>
    {
        public string ShortCode { get; set; }
    }

    public class GetUserUrlsQuery : IRequest<IEnumerable<ShortenedUrlDto>>
    {
        public string UserId { get; set; }
    }
}

using System;
using UrlShortener.Domain.ValueObjects.IDs;

namespace UrlShortener.Domain.Entities
{
    public class ShortenedUrl
    {
        public ShortenedUrlId Id { get; set; }
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        
        public User Creator { get; set; }
        
        public UserId CreatorId { get; set; }
    }
}

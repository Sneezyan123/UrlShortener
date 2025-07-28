using System;

namespace UrlShortener.Domain.Entities
{
    public class AboutPage
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; }
    }
}

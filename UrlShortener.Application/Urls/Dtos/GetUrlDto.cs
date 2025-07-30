namespace UrlShortener.Applicationm.Urls.Dtos;

public class GetUrlDto
{
    public Guid Id { get; set; }
    public string OriginalUrl { get; set; }
    public string ShortCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatorEmail { get; set; }
}
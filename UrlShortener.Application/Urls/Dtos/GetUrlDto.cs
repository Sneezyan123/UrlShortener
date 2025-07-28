namespace UrlShortener.Applicationm.Urls.Dtos;

public class GetUrlDto
{
    public string Id { get; set; }
    public string OriginalUrl { get; set; }
    public string ShortCode { get; set; }
    public string CreatedAt { get; set; }
    public string CreatorEmail { get; set; }
}
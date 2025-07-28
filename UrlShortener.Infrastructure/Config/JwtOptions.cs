namespace UrlShortener.Infrastructure.Config;

public class JwtOptions
{
    public string SecretKey { get; set; }
    
    public int ExpiresIn { get; set; }
}
namespace UrlShortener.Application.ViewModels
{
    public class AboutViewModel
    {
        public string Content { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; } = string.Empty;
        public bool CanEdit { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace UrlShortener.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [Display(Name = "Email")]
    public string Username { get; set; } = string.Empty;
        
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
        
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
        
    public string? ReturnUrl { get; set; }
}
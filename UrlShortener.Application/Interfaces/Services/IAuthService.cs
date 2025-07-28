namespace UrlShortener.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<bool> ValidateUserAsync(string username, string password);
        Task<bool> IsInRoleAsync(string username, string role);
        Task<bool> CreateUserAsync(string username, string password, string role = "User");
    }
}

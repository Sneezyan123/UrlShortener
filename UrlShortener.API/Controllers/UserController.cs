using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediatR;
using UrlShortener.Application.ViewModels;
using UrlShortener.Applicationm.Users.Commands.CreateUserCommand;
using UrlShortener.Applicationm.Users.Commands.LoginUserCommand;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly ISender _sender;
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository, ISender sender)
        {
            _userRepository = userRepository;
            _sender = sender;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new CreateUserCommandRequest());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CreateUserCommandRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var command = new CreateUserCommand(request.Email, request.Password);
            var result = await _sender.Send(command);
            
            if (result.IsFailed)
            {
                ModelState.AddModelError(string.Empty, result.Errors.First().Message);
                return View(request);
            }

            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction(nameof(Login));
        }
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }
            
            var viewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var command = new LoginUserCommand(model.Username, model.Password);
            var result = await _sender.Send(command);

            if (result.IsFailed)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.Value.Value.ToString()),
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.Email, model.Username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
            var authProperties = new AuthenticationProperties 
            { 
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }
        

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet("user/status")]
        [AllowAnonymous]
        public IActionResult GetAuthStatus()
        {
            var cookies = Request.Cookies.Select(c => new { Name = c.Key, Value = c.Value }).ToList();
            
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                
                return Ok(new
                {
                    isAuthenticated = true,
                    userId = userId,
                    userName = userName ?? email,
                    email = email,
                    cookies = cookies
                });
            }

            return Ok(new { 
                isAuthenticated = false,
                cookies = cookies
            });
        }

        [HttpPost("user/logout")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth", new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(-1),
                RedirectUri = null
            });
            
            Response.Cookies.Delete(".AspNetCore.Cookies");
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                Request.Headers["Content-Type"].ToString().Contains("application/json"))
            {
                return Ok(new { success = true });
            }
            
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("user/logout")]
        [Authorize] 
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync("CookieAuth", new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(-1),
                RedirectUri = null
            });
            
            Response.Cookies.Delete(".AspNetCore.Cookies");
            return RedirectToAction("Index", "Home");
        }
    }
}
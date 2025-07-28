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

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(CreateUserCommandRequest request)
        {
            var command = new CreateUserCommand(request.Email, request.Password);
            var result = await _sender.Send(command);
            
            if (result.IsFailed)
            {
                ModelState.AddModelError(string.Empty, result.Errors.First().Message);
                return View(request);
            }

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
                new Claim(ClaimTypes.Name, model.Username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
            await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = model.RememberMe });

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

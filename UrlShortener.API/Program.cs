using UrlShortener.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
    options.Cookie.Name = "__RequestVerificationToken";
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

builder
    .AddConfiguration()
    .AddExceptionHandlers()
    .AddServices()
    .AddEfCoreDatabase()
    .AddDapper()
    .AddAuth();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.Request.Method == "GET")
    {
        var antiforgery = context.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
        var tokenSet = antiforgery.GetAndStoreTokens(context);
        if (tokenSet.RequestToken != null)
        {
            context.Response.Headers["X-CSRF-TOKEN"] = tokenSet.RequestToken;
        }
    }
    await next();
});

app.MapControllerRoute(name: "api", pattern: "api/{controller}/{action=Index}/{id?}");
app.MapControllerRoute(name: "redirect", pattern: "s/{shortCode}", defaults: new { controller = "Redirect", action = "RedirectToUrl" });
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}").WithStaticAssets();
app.MapStaticAssets();

app.Run();
using UrlShortener.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Додаємо підтримку Antiforgery токенів
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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseRouting();
app.MapStaticAssets();

// Важливо: порядок middleware має значення
app.UseAuthentication();
app.UseAuthorization();

// Додаємо підтримку antiforgery токенів
app.Use(async (context, next) =>
{
    // Додаємо CSRF токен в заголовки для всіх запитів
    if (context.Request.Method == "GET")
    {
        var antiforgery = context.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
        var tokenSet = antiforgery.GetAndStoreTokens(context);
        if (tokenSet.RequestToken != null)
        {
            context.Response.Headers.Add("X-CSRF-TOKEN", tokenSet.RequestToken);
        }
    }
    await next();
});

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
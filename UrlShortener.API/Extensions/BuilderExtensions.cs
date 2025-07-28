using System.Reflection;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UrlShortener.Applicationm;
using UrlShortener.Applicationm.Abstractions;
using UrlShortener.Applicationm.Pipeline;
using UrlShortener.Domain.DomainAbstractions;
using UrlShortener.Domain.Repositories;
using UrlShortener.Handlers;
using UrlShortener.Infrastructure;
using UrlShortener.Infrastructure.Config;
using UrlShortener.Infrastructure.Repositories;
using UrlShortener.Infrastructure.Services;

namespace UrlShortener.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(AssemblyApplicationReference.Assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            }
        );
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddValidatorsFromAssembly(AssemblyInfrastructureReference.Assembly);
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return builder;
    }
    
    public static WebApplicationBuilder AddExceptionHandlers(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        return builder;
    }
    
    public static WebApplicationBuilder AddConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services
            .Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
        return builder;
    }
    
    public static WebApplicationBuilder AddAuth(this WebApplicationBuilder builder)
    {
        var jwtOptions = new JwtOptions();
        var jwtOptionsConfig = builder.Configuration.GetSection("JwtOptions");
        jwtOptionsConfig.Bind(jwtOptions);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "CookieAuth";
            options.DefaultChallengeScheme = "CookieAuth";
            options.DefaultSignInScheme = "CookieAuth";
        })
        .AddCookie("CookieAuth", options =>
        {
            options.LoginPath = "/User/Login";
            options.LogoutPath = "/User/Logout";
            options.AccessDeniedPath = "/User/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.SlidingExpiration = true;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
            };
        });
        
        builder.Services.AddAuthorization();
        return builder;
    }
    
    public static WebApplicationBuilder AddEfCoreDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApiDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnect"));
        });
        return builder;
    }
}
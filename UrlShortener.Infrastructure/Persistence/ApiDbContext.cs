using System.Reflection;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure;

public class ApiDbContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<ShortenedUrl> ShortenedUrls { get; set; }
    public ApiDbContext(DbContextOptions<ApiDbContext> options): base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyInfrastructureReference.Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
}
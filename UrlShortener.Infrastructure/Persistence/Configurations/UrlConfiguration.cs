using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.ValueObjects.IDs;

namespace UrlShortener.Infrastructure.Persistence.Configurations;

public class UrlConfiguration : IEntityTypeConfiguration<ShortenedUrl>
{
    public void Configure(EntityTypeBuilder<ShortenedUrl> builder)
    {
        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => new ShortenedUrlId(value));

        builder.HasKey(u => u.Id);

        builder.Property(u => u.OriginalUrl)
            .IsRequired();
        

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(u => u.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => u.ShortUrl).IsUnique();
    }
}

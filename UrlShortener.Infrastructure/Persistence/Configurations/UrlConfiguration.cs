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
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(u => u.ShortCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.CreatorId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.HasOne(u => u.Creator)
            .WithMany(u => u.MyShortenedUrls)
            .HasForeignKey(u => u.CreatorId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasIndex(u => u.ShortCode)
            .IsUnique();

        builder.HasIndex(u => u.CreatedAt);
    }
}
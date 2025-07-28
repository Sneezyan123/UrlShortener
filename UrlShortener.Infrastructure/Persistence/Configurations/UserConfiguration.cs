using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.ValueObjects;
using UrlShortener.Domain.ValueObjects.IDs;

namespace UrlShortener.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {

        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value));
        

        builder
            .ComplexProperty(m => m.Email, email =>
            {
                email
                    .Property(e => e.Value)
                    .IsRequired();
            });

        builder
            .ComplexProperty(m => m.Password, password =>
            {
                password
                    .Property(p => p.Value)
                    .IsRequired();
            });
    }
}

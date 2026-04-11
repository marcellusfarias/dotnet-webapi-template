using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBuyingList.Domain.Constants;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Infrastructure.Persistence.Configurations;

internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.Property(p => p.Id).UseIdentityAlwaysColumn();
        builder.Property(p => p.UserId).IsRequired();
        builder.Property(p => p.TokenHash).IsRequired().HasMaxLength(FieldLengths.REFRESH_TOKEN_HASH_MAX_LENGTH);
        builder.Property(p => p.ExpiresAt).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired().HasDefaultValueSql("NOW()");
        builder.Property(p => p.IsRevoked).IsRequired().HasDefaultValue(false);

        builder.HasKey(p => p.Id).HasName("PK_refresh_tokens_id");
        builder.HasIndex(p => p.TokenHash).IsUnique();

        builder.HasOne(p => p.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

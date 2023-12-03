using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBuyingList.Application.Common.Services;
using MyBuyingList.Domain.Constants;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Infrastructure.Persistence.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.Property(p => p.Id).UseIdentityAlwaysColumn();
        builder.Property(p => p.UserName).HasMaxLength(FieldLengths.USER_USERNAME_MAX_LENGTH).IsRequired();
        builder.Property(p => p.Email).IsRequired().HasMaxLength(FieldLengths.USER_EMAIL_MAX_LENGTH);
        builder.Property(p => p.Password).IsRequired().HasMaxLength(FieldLengths.USER_PASSWORD_MAX_LENGTH);
        builder.Property(p => p.CreatedAt).IsRequired().HasDefaultValueSql("NOW()");
        builder.Property(p => p.Active).IsRequired().HasDefaultValueSql("FALSE");

        builder.HasKey(p => p.Id).HasName("PK_users_id");
        builder.HasIndex(p => p.Email).IsUnique(true);
        builder.HasIndex(p => p.UserName).IsUnique(true);
        builder.HasMany(p => p.GroupsCreatedBy)
            .WithOne(g => g.User)
            .HasForeignKey(g => g.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(p => p.BuyingListCreatedBy)
            .WithOne(bl => bl.UserCreated)
            .HasForeignKey(bl => bl.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t => 
            t.HasCheckConstraint("CHK_Username_MinLength", 
            $"(length(user_name) >= {FieldLengths.USER_USERNAME_MIN_LENGTH})"));

        PasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
        builder.HasData(
            new User 
            { 
                Id = 1, 
                UserName = "admin", 
                Email = "marcelluscfarias@gmail.com", 
                Password = "$2a$16$CZ18qbFWtcoAY6SnsqNYnO1H.D3It5TTD6uuhTFyjge5I/n5SRLKe", 
                Active = true 
            });
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Infrastructure.Persistence.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.Property(p => p.Id).UseIdentityColumn();
        builder.Property(p => p.UserName).HasMaxLength(256).IsRequired();
        builder.Property(p => p.Email).IsRequired().HasMaxLength(256);
        builder.Property(p => p.Password).IsRequired().HasMaxLength(32);
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

        builder.HasData(
            new User { Id = 1, UserName = "admin", Email = "marcelluscfarias@gmail.com", Password = "123" });
    }
}

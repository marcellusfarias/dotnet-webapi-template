using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.Property(p => p.UserId).UseIdentityColumn();
        builder.Property(p => p.UserName).HasMaxLength(256).IsRequired();
        builder.Property(p => p.Email).IsRequired().HasMaxLength(256);
        builder.Property(p => p.Password).IsRequired().HasMaxLength(32);
        builder.Property(p => p.CreatedAt).IsRequired().HasMaxLength(32).HasDefaultValue(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc));

        builder.HasKey(p => p.UserId).HasName("PK_users_id");
        builder.HasIndex(p => p.Email).IsUnique(true);
        builder.HasIndex(p => p.UserName).IsUnique(true);

        builder.HasData(
            new User { UserId = 1, UserName = "admin", Email = "marcelluscfarias@gmail.com", Password = "123" });
    }
}

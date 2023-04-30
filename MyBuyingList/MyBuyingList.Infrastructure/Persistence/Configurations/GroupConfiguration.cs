using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Infrastructure.Persistence.Configurations;

internal class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("groups");

        builder.Property(g => g.Id).UseIdentityColumn();
        builder.Property(g => g.GroupName).HasMaxLength(256).IsRequired();
        builder.Property(g => g.CreatedAt).IsRequired().HasDefaultValueSql("NOW()");
        builder.Property(g => g.CreatedBy).IsRequired();

        builder.HasKey(g => g.Id);
        builder.HasIndex(g => g.GroupName).IsUnique(true);
        builder.HasOne(g => g.User)
            .WithMany(u => u.GroupsCreatedBy)
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(g => g.BuyingLists)
            .WithOne(bl => bl.Group)
            .HasForeignKey(bl => bl.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Group { Id = 1, GroupName = "Default", CreatedBy = 1 }); ;
    }
}

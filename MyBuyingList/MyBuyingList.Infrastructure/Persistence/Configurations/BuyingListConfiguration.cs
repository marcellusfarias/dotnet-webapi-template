using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Infrastructure.Persistence.Configurations;

internal class BuyingListConfiguration : IEntityTypeConfiguration<BuyingList>
{
    public void Configure(EntityTypeBuilder<BuyingList> builder)
    {
        builder.ToTable("buying_lists");

        builder.Property(bl => bl.Id).UseIdentityAlwaysColumn();
        builder.Property(bl => bl.Name).HasMaxLength(256).IsRequired();
        builder.Property(bl => bl.GroupId).IsRequired();
        builder.Property(g => g.CreatedAt).IsRequired().HasDefaultValueSql("NOW()");
        builder.Property(g => g.CreatedBy).IsRequired();

        builder.HasKey(bl => bl.Id);
        builder.HasOne(bl => bl.Group)
            .WithMany(g => g.BuyingLists)
            .HasForeignKey(bl => bl.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(bl => bl.UserCreated)
            .WithMany(u => u.BuyingListCreatedBy)
            .HasForeignKey(bl => bl.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new BuyingList { Id = 1, Name = "Default", GroupId = 1, CreatedBy = 1 }); ;
    }
}
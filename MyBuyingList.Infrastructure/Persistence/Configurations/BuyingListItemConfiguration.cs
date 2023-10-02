using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Infrastructure.Persistence.Configurations;

internal class BuyingListItemConfiguration : IEntityTypeConfiguration<BuyingListItem>
{
    public void Configure(EntityTypeBuilder<BuyingListItem> builder)
    {
        builder.ToTable("buying_list_items");

        builder.Property(bli => bli.Id).UseIdentityAlwaysColumn();
        builder.Property(bli => bli.Description).HasMaxLength(256).IsRequired();
        builder.Property(bli => bli.Completed).IsRequired();

        builder.HasKey(bl => bl.Id);
        builder.HasOne(bli => bli.BuyingList)
            .WithMany(bl => bl.Items)
            .HasForeignKey(bli => bli.BuyingListId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new BuyingListItem { Id = 1, Description = "Default", Completed = false, BuyingListId = 1 }); ;
    }
}
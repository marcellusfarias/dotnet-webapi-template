﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using MyBuyingList.Infrastructure.Auth.Constants;

namespace MyBuyingList.Infrastructure.Persistence.Configurations;

public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.ToTable("policies");

        builder.Property(x => x.Id).UseIdentityAlwaysColumn();
        builder.Property(policy => policy.Name).HasMaxLength(256).IsRequired();
        builder.HasIndex(policy => policy.Name).IsUnique(true);

        builder.HasMany(policy => policy.RolePolicies)
            .WithOne(rolePolicy => rolePolicy.Policy)
            .HasForeignKey(rolePolicy => rolePolicy.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(Policies.GetValues());
    }
}
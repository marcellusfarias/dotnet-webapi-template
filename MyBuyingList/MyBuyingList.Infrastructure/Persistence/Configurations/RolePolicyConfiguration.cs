using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Infrastructure.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Infrastructure.Persistence.Configurations;

public class RolePolicyConfiguration : IEntityTypeConfiguration<RolePolicy>
{
    public void Configure(EntityTypeBuilder<RolePolicy> builder)
    {
        builder.ToTable("role_policies");
        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.RoleId, x.PolicyId });

        builder.HasOne(rolePolicy => rolePolicy.Policy)
            .WithMany(policy => policy.RolePolicies)
            .HasForeignKey(rolePolicy => rolePolicy.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(rolePolicy => rolePolicy.Role)
            .WithMany(role => role.RolePolicies)
            .HasForeignKey(rolePolicy => rolePolicy.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}


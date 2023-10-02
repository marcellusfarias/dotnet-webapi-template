using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Infrastructure.Persistence.Configurations;

public class RolePolicyConfiguration : IEntityTypeConfiguration<RolePolicy>
{
    public void Configure(EntityTypeBuilder<RolePolicy> builder)
    {
        builder.ToTable("role_policies");
        builder.Property(p => p.Id).UseIdentityAlwaysColumn();
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


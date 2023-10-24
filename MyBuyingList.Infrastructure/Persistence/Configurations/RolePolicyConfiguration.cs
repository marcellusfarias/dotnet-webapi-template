using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Domain.Constants;

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

        // Adding RolePolicies for Admin role.
        var policies = Policies.GetValues();
        var rolePolicies = new List<RolePolicy>();
        int i = 1;

        foreach(var policy in policies)
        {
            rolePolicies.Add(new RolePolicy { Id = i, RoleId = 1, PolicyId = policy.Id });
            i++;
        }

        builder.HasData(rolePolicies);
    }
}


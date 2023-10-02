using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Infrastructure.Auth.Constants;

namespace MyBuyingList.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.Property(p => p.Id).UseIdentityAlwaysColumn();
        builder.Property(role => role.Name).HasMaxLength(256).IsRequired();
        builder.HasKey(p => p.Id).HasName("PK_roles_id");

        builder.HasIndex(role => role.Name).IsUnique(true);
        builder.HasMany(role => role.UserRoles)
            .WithOne(userRole => userRole.Role)
            .HasForeignKey(userRole => userRole.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(role => role.RolePolicies)
            .WithOne(rolePolicy => rolePolicy.Role)
            .HasForeignKey(rolePolicy => rolePolicy.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(Roles.GetValues());
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Infrastructure.Authentication;

namespace MyBuyingList.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.Property(role => role.Id).UseIdentityColumn();
        builder.Property(role => role.Name).HasMaxLength(256).IsRequired();

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

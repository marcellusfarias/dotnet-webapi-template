using Microsoft.AspNetCore.Authorization;

namespace MyBuyingList.Infrastructure.Auth.AuthorizationHandlers;

internal class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
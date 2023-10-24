using Microsoft.AspNetCore.Authorization;

namespace MyBuyingList.Web.Middlewares.Authorization;

internal class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
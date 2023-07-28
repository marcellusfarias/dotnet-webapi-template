using Microsoft.AspNetCore.Authorization;

namespace MyBuyingList.Infrastructure.Auth.AuthorizationHandlers;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
        : base(policy: permission)
    {

    }
}
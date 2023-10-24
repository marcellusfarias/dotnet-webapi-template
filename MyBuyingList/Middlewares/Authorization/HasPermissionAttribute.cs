using Microsoft.AspNetCore.Authorization;

namespace MyBuyingList.Web.Middlewares.Authorization;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
        : base(policy: permission)
    {

    }
}
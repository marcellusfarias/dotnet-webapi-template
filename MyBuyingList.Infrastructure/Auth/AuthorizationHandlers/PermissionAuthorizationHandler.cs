using Microsoft.AspNetCore.Authorization;
using MyBuyingList.Infrastructure.Auth.AuthorizationHandlers;
using MyBuyingList.Infrastructure.Auth.Constants;

namespace MyBuyingList.Infrastructure.Auth.AuthorizationHandlers;

internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var permissions = context.User.Claims.Where(x => x.Type == CustomClaims.Permissions).Select(x => x.Value);
        if (permissions.Contains(requirement.Permission))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace MyBuyingList.Infrastructure.Auth.AuthorizationHandlers;

internal class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
    }

    // Im not sure how to use this yet. Got from Milan's video: https://www.youtube.com/watch?v=SUyFPp6BPV0
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);

        if (policy is not null)
        {
            return policy;
        }

        return new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();
    }
}
namespace OSN.Application;

public static class RoleHierarchy
{
    // POLICY: ROLE
    // Use Authorize(Policy=".."), NOT Authorize(Roles="..") FOR HIERARCHY.
    // Hierarchy is implemented in Program.cs for Controller level authorization.
    // AND AuthorizationBehavior.cs for Command/Query level authorization.
    public static readonly string UserPolicy = "UserPolicy";
    public static readonly string AdminPolicy = "AdminPolicy";
    public static readonly string SuperAdminPolicy = "SuperAdminPolicy";
    public static readonly string DefaultPolicy = UserPolicy;

    public static readonly IReadOnlyDictionary<string, string[]> _hierarchy = new Dictionary<string, string[]>
    {
        [UserPolicy] = ["User", "Admin", "SuperAdmin"],
        [AdminPolicy] = ["Admin", "SuperAdmin"],
        [SuperAdminPolicy] = ["SuperAdmin"]
    };


    public static IEnumerable<string> GetEffectiveRoles(string policy)
    {
        return _hierarchy.TryGetValue(policy, out var roles)
            ? roles
            : throw new ArgumentException($"Unknown policy: {policy}");
    }

    public static bool HasPermission(string userRole, string requiredPolicy)
    {
        return GetEffectiveRoles(requiredPolicy).Contains(userRole);
    }
}
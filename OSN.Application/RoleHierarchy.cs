namespace OSN.Application;

public static class RoleHierarchy
{
    // POLICY: ROLE
    // Use Authorize(Policy=".."), NOT Authorize(Roles="..") FOR HIERARCHY.
    // Hierarchy is implemented in Program.cs for Controller level authorization.
    // AND AuthorizationBehavior.cs for Command/Query level authorization.
    public const string GuestPolicy = "GuestPolicy";
    public const string UserPolicy = "UserPolicy";
    public const string AdminPolicy = "AdminPolicy";
    public const string SuperAdminPolicy = "SuperAdminPolicy";
    public const string DefaultPolicy = UserPolicy;

    public const string GuestRole = "Guest";
    public const string UserRole = "User";
    public const string AdminRole = "Admin";
    public const string SuperAdminRole = "SuperAdmin";

    public static readonly IReadOnlyDictionary<string, string[]> _hierarchy = new Dictionary<string, string[]>
    {
        [GuestPolicy] = [GuestRole, UserRole, AdminRole, SuperAdminRole],
        [UserPolicy] = [UserRole, AdminRole, SuperAdminRole],
        [AdminPolicy] = [AdminRole, SuperAdminRole],
        [SuperAdminPolicy] = [SuperAdminRole]
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
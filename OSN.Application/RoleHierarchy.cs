namespace OSN.Application;

public static class RoleHierarchy
{
    public static readonly IReadOnlyDictionary<string, string[]> _hierarchy = new Dictionary<string, string[]>
    {
        ["User"] = ["User", "Admin", "SuperAdmin"],
        ["Admin"] = ["Admin", "SuperAdmin"],
        ["SuperAdmin"] = ["SuperAdmin"]
    };

    public static readonly string DefaultRole = "User";

    public static IEnumerable<string> GetEffectiveRoles(string role)
    {
        return _hierarchy.TryGetValue(role, out var roles)
            ? roles
            : throw new ArgumentException($"Unknown role: {role}");
    }

    public static bool HasPermission(string userRole, string requiredRole)
    {
        return GetEffectiveRoles(requiredRole).Contains(userRole);
    }
}
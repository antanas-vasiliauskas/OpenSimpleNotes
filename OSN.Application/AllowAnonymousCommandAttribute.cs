namespace OSN.Application;

[AttributeUsage(AttributeTargets.Class)]
public class AllowAnonymousCommandAttribute: Attribute { }

// Goes on commands/queries. Behavior defined by AuthorizationBehavior.cs
// AuthorizationBehavior:
// By default, uses RoleHierarchy.DefaultPolicy (UserPolicy).
// If [AllowAnonymousCommand] is present, skips authorization check.
// Can add [AuthorizeCommand(Policy = RoleHierarchy.AdminPolicy)] to require higer role.
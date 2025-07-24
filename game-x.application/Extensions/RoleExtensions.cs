namespace game_x.application.Extensions;

public static class RoleExtensions
{
    public static bool IsAdmin(this IEnumerable<string> roles)
    {
        return roles.Any(role => role.Equals(AppRoles.Admin, StringComparison.OrdinalIgnoreCase));
    }

    public static bool IsStaff(this IEnumerable<string> roles)
    {
        return roles.Any(role => role.Equals(AppRoles.Staff, StringComparison.OrdinalIgnoreCase));
    }

    public static bool IsUser(this IEnumerable<string> roles)
    {
        return roles.Any(role => role.Equals(AppRoles.User, StringComparison.OrdinalIgnoreCase));
    }

    public static bool IsRoot(this IEnumerable<string> roles)
    {
        return roles.Any(role => role.Equals(AppRoles.Root, StringComparison.OrdinalIgnoreCase));
    }

    public static bool HasRole(this IEnumerable<string> roles, string role)
    {
        return roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}